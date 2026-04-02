using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Data;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.Requests;

namespace Odasoft.XBOL.Business.Services;

/// <summary>
/// Unified seat management operations for both EventSchedules and Seasons.
/// Resolves Seats.io external keys to the correct entity type and delegates
/// to the Ticketing API for Seats.io state changes, then mirrors ForSale
/// state in the local database.
/// </summary>
public class SeatManagementService(
    XBOLDbContext dbContext,
    ITicketingClient ticketingClient,
    ILogger<SeatManagementService> logger)
{
    /// <summary>
    /// Returns detail for the seat selector page. Tries EventSchedule first, then Season.
    /// </summary>
    public async Task<SeatManagementDetailDTO?> GetDetailAsync(string externalKey)
    {
        var scheduleResult = await GetScheduleDetailAsync(externalKey);
        if (scheduleResult is not null) return scheduleResult;

        return await GetSeasonDetailAsync(externalKey);
    }

    /// <summary>
    /// Blocks seats: sets extraData (reason + color) first, then forSale=false.
    /// Order matters — prevents the renderer from showing a blocked seat without color context.
    /// </summary>
    public async Task<bool> BlockSeatsAsync(string externalKey, BlockSeatsRequest request)
    {
        var resolved = await ResolveExternalKeyAsync(externalKey);
        if (resolved is null) return false;

        // Set extraData first so the renderer has color context before the seat appears blocked
        await ticketingClient.UpdateSeatExtraDataAsync(new UpdateSeatExtraDataRequest
        {
            EventKey = externalKey,
            SeatKeys = request.SeatKeys,
            ExtraData = new Dictionary<string, object>
            {
                ["reason"] = request.Reason,
                ["color"] = request.Color
            }
        });

        await ticketingClient.SetForSaleAsync(new SetForSaleRequest
        {
            EventKey = externalKey,
            SeatKeys = request.SeatKeys,
            ForSale = false
        });

        // Mirror Seats.io state in local DB for availability counts
        await UpdateLocalForSaleAsync(resolved, request.SeatKeys, forSale: false);

        return true;
    }

    /// <summary>
    /// Unblocks seats: overwrites extraData with unblock reason (replaces block reason + color),
    /// then sets forSale=true. Same pattern as block — extraData first, then forSale change.
    /// </summary>
    public async Task<bool> UnblockSeatsAsync(string externalKey, UnblockSeatsRequest request)
    {
        var resolved = await ResolveExternalKeyAsync(externalKey);
        if (resolved is null) return false;

        // Overwrite extraData with unblock reason (replaces block reason + color)
        await ticketingClient.UpdateSeatExtraDataAsync(new UpdateSeatExtraDataRequest
        {
            EventKey = externalKey,
            SeatKeys = request.SeatKeys,
            ExtraData = new Dictionary<string, object>
            {
                ["reason"] = request.Reason
            }
        });

        await ticketingClient.SetForSaleAsync(new SetForSaleRequest
        {
            EventKey = externalKey,
            SeatKeys = request.SeatKeys,
            ForSale = true
        });

        await UpdateLocalForSaleAsync(resolved, request.SeatKeys, forSale: true);

        return true;
    }

    /// <summary>
    /// Updates reason and color on already-blocked seats. No forSale change, no local DB change.
    /// </summary>
    public async Task<bool> EditBlockedSeatsAsync(string externalKey, BlockSeatsRequest request)
    {
        var resolved = await ResolveExternalKeyAsync(externalKey);
        if (resolved is null) return false;

        await ticketingClient.UpdateSeatExtraDataAsync(new UpdateSeatExtraDataRequest
        {
            EventKey = externalKey,
            SeatKeys = request.SeatKeys,
            ExtraData = new Dictionary<string, object>
            {
                ["reason"] = request.Reason,
                ["color"] = request.Color
            }
        });

        return true;
    }

    /// <summary>
    /// Resolves a Seats.io external key to an entity. Tries EventSchedule.ExternalEventKey
    /// first, then Season.ExternalSeasonKey. Returns null if neither matches.
    /// </summary>
    public async Task<ResolvedKey?> ResolveExternalKeyAsync(string externalKey)
    {
        var scheduleId = await dbContext.EventSchedules
            .AsNoTracking()
            .Where(s => s.ExternalEventKey == externalKey)
            .Select(s => (long?)s.Id)
            .FirstOrDefaultAsync();

        if (scheduleId is not null)
            return new ResolvedKey(BookableUnitType.Schedule, scheduleId.Value);

        var seasonId = await dbContext.Seasons
            .AsNoTracking()
            .Where(s => s.ExternalSeasonKey == externalKey && s.DeletedAt == null)
            .Select(s => (long?)s.Id)
            .FirstOrDefaultAsync();

        if (seasonId is not null)
            return new ResolvedKey(BookableUnitType.Season, seasonId.Value);

        return null;
    }

    /// <summary>
    /// Mirrors Seats.io ForSale state in the local DB. Finds EventSeat or SeasonSeat
    /// records by ExternalSeatObjectKey within the resolved entity, then sets ForSale.
    /// Failures are logged but not thrown — Seats.io is the source of truth, local state
    /// will be stale until reconciled.
    /// </summary>
    private async Task UpdateLocalForSaleAsync(ResolvedKey resolved, IList<string> seatKeys, bool forSale)
    {
        try
        {
            if (resolved.Type == BookableUnitType.Schedule)
            {
                var seats = await dbContext.EventSeats
                    .Where(es => es.EventSection.EventScheduleId == resolved.EntityId)
                    .Where(es => seatKeys.Contains(es.ExternalSeatObjectKey))
                    .ToListAsync();

                LogMismatch(seats.Count, seatKeys.Count, resolved);

                foreach (var seat in seats)
                    seat.ForSale = forSale;
            }
            else
            {
                var seats = await dbContext.SeasonSeats
                    .Where(ss => ss.SeasonSection.SeasonId == resolved.EntityId)
                    .Where(ss => seatKeys.Contains(ss.ExternalSeatObjectKey))
                    .ToListAsync();

                LogMismatch(seats.Count, seatKeys.Count, resolved);

                foreach (var seat in seats)
                    seat.ForSale = forSale;
            }

            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Seats.io updated successfully but local DB update failed for {Type} {EntityId}. ForSale state is stale",
                resolved.Type, resolved.EntityId);
        }
    }

    private void LogMismatch(int found, int requested, ResolvedKey resolved)
    {
        if (found < requested)
        {
            logger.LogWarning(
                "Seats.io updated but only {Found}/{Requested} seat keys found locally for {Type} {EntityId}",
                found, requested, resolved.Type, resolved.EntityId);
        }
    }

    #region Detail queries

    /// <summary>
    /// Builds detail DTO for an EventSchedule. Prices combine two sources:
    /// seat-level overrides (grouped by PriceOverride, returns object keys) and
    /// category-level prices (grouped by zone external key, returns min section price).
    /// </summary>
    private async Task<SeatManagementDetailDTO?> GetScheduleDetailAsync(string externalKey)
    {
        var scheduleId = await dbContext.EventSchedules
            .AsNoTracking()
            .Where(s => s.ExternalEventKey == externalKey)
            .Select(s => (long?)s.Id)
            .FirstOrDefaultAsync();

        if (scheduleId is null) return null;

        // Seat-level: individual seats with PriceOverride, grouped by price value
        var seatsPrice = await dbContext.EventSeats
            .AsNoTracking()
            .Where(es => es.EventSection.EventScheduleId == scheduleId)
            .Where(es => es.PriceOverride != null)
            .GroupBy(es => es.PriceOverride)
            .Select(g => new SeatsIoPriceDTO
            {
                Category = null,
                Objects = g.Select(x => x.ExternalSeatObjectKey).ToArray(),
                OriginalPrice = null,
                Price = g.Key ?? 0m,
                Fee = null
            })
            .ToListAsync();

        // Category-level: min section price per zone (for Seats.io category pricing)
        var categoriesPrice = await dbContext.EventSections
            .AsNoTracking()
            .Where(es => es.EventScheduleId == scheduleId)
            .Where(es => es.BaseSection.BaseZone.ExternalZoneKey != null)
            .GroupBy(es => es.BaseSection.BaseZone.ExternalZoneKey)
            .Select(g => new SeatsIoPriceDTO
            {
                Category = g.Key,
                Objects = null,
                OriginalPrice = null,
                Price = g.Min(es => es.Price) ?? 0,
                Fee = null
            })
            .ToListAsync();

        var prices = seatsPrice.Concat(categoriesPrice).ToList();

        return await dbContext.EventSchedules
            .AsNoTracking()
            .Where(s => s.Id == scheduleId)
            .Select(s => new SeatManagementDetailDTO
            {
                Type = BookableUnitType.Schedule,
                ExternalKey = s.ExternalEventKey,
                Name = s.Event.Name,
                Subtitle = s.Event.Subtitle,
                BannerImageUrl = s.Event.BannerImageUrl,
                VenueName = s.Event.VenueMap.Venue.Name,
                StartDate = s.StartDateTime,
                EndDate = s.EndDateTime,
                TotalSeats = s.Sections.Sum(sec => sec.TotalSeats),
                AvailableSeats = s.Sections.Sum(sec => sec.AvailableSeats),
                Prices = prices
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Builds detail DTO for a Season. Same pricing pattern as schedule.
    /// VenueName is derived from the first Event linked to the Season since
    /// Seasons don't have a direct VenueMap reference.
    /// </summary>
    private async Task<SeatManagementDetailDTO?> GetSeasonDetailAsync(string externalKey)
    {
        var seasonId = await dbContext.Seasons
            .AsNoTracking()
            .Where(s => s.ExternalSeasonKey == externalKey && s.DeletedAt == null)
            .Select(s => (long?)s.Id)
            .FirstOrDefaultAsync();

        if (seasonId is null) return null;

        var seatsPrice = await dbContext.SeasonSeats
            .AsNoTracking()
            .Where(ss => ss.SeasonSection.SeasonId == seasonId)
            .Where(ss => ss.PriceOverride != null)
            .GroupBy(ss => ss.PriceOverride)
            .Select(g => new SeatsIoPriceDTO
            {
                Category = null,
                Objects = g.Select(x => x.ExternalSeatObjectKey).ToArray(),
                OriginalPrice = null,
                Price = g.Key ?? 0m,
                Fee = null
            })
            .ToListAsync();

        var categoriesPrice = await dbContext.SeasonSections
            .AsNoTracking()
            .Where(ss => ss.SeasonId == seasonId)
            .Where(ss => ss.BaseSection.BaseZone.ExternalZoneKey != null)
            .GroupBy(ss => ss.BaseSection.BaseZone.ExternalZoneKey)
            .Select(g => new SeatsIoPriceDTO
            {
                Category = g.Key,
                Objects = null,
                OriginalPrice = null,
                Price = g.Min(ss => ss.Price) ?? 0,
                Fee = null
            })
            .ToListAsync();

        var prices = seatsPrice.Concat(categoriesPrice).ToList();

        return await dbContext.Seasons
            .AsNoTracking()
            .Where(s => s.Id == seasonId)
            .Select(s => new SeatManagementDetailDTO
            {
                Type = BookableUnitType.Season,
                ExternalKey = s.ExternalSeasonKey,
                Name = s.Name,
                Subtitle = null,
                BannerImageUrl = s.BannerImageUrl,
                VenueName = dbContext.Events
                    .Where(e => e.SeasonId == s.Id)
                    .Select(e => e.VenueMap.Venue.Name)
                    .FirstOrDefault(),
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                TotalSeats = s.SeasonSections.Sum(sec => sec.TotalSeats),
                AvailableSeats = s.SeasonSections.Sum(sec => sec.AvailableSeats),
                Prices = prices
            })
            .FirstOrDefaultAsync();
    }

    #endregion

    public record ResolvedKey(BookableUnitType Type, long EntityId);
}
