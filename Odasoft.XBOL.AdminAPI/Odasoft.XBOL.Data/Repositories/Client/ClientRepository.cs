using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Requests.Filters;
using XBOL.Admin.Core.DTO;

namespace Odasoft.XBOL.Data.Repositories.Client
{
    public class ClientRepository(XBOLDbContext dbContext) : BaseRepository<Models.Client>(dbContext)
    {
        public async Task<ClientSeasonEvent?> GetClientSeasonEventInfoAsync(ClientFilter filter)
        {
            ClientSeasonEvent result = new() { AlreadyRenewed = false, CanRenovate = false };

            var client = await FindClientAsync(filter);

            if (client is null)
            {
                return null;
            }

            result.ClientContact = new()
            {
                Id = client.Id,
                CountryPhoneISO = client.User?.CountryPhoneISO ?? "",
                PhoneNumber = client.PhoneNumber ?? "",
                Email = client.Email ?? "",
                Name = string.IsNullOrWhiteSpace(client.BusinessName) ? (client.FullName ?? "") : client.BusinessName,
                LastName = ""
            };

            var season = await dbContext.Seasons.FirstOrDefaultAsync(s => s.Id == filter.SeasonId);

            if (season is null)
            {
                return result;
            }

            result.SeasonKey = season.ExternalSeasonKey;

            var passSeasons = await GetClientSeasonPassSeasonIdsAsync(client.Id, season);

            ApplyRenewalRules(result, season, passSeasons);

            return result;
        }

        private async Task<Models.Client?> FindClientAsync(ClientFilter filter)
        {
            var email = filter.Email?.Trim().ToLower();

            return await dbContext.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c =>
                    (!string.IsNullOrWhiteSpace(email) && (c.Email == null ? "" : c.Email.ToLower().Trim()) == email)
                    || (!string.IsNullOrEmpty(filter.PhoneNumber)
                        && c.PhoneNumber == filter.PhoneNumber
                        && c.User!.CountryPhoneISO == filter.CountryPhoneISO)
                );
        }

        private async Task<HashSet<long>> GetClientSeasonPassSeasonIdsAsync(long clientId, Models.Season currentSeason)
        {
            var idsToCheck = GetSeasonIdsToCheck(currentSeason);

            var seasonIds = await dbContext.SeasonPasses
                .Where(sp => sp.ClientId == clientId && idsToCheck.Contains(sp.SeasonId))
                .Select(sp => sp.SeasonId)
                .ToListAsync();

            return seasonIds.ToHashSet();
        }

        private static List<long> GetSeasonIdsToCheck(Models.Season currentSeason)
        {
            var ids = new List<long>(capacity: 2) { currentSeason.Id };

            if (currentSeason.PreviousSeasonId is long prevId)
            {
                ids.Add(prevId);
            }

            return ids;
        }

        private static void ApplyRenewalRules(ClientSeasonEvent result, Models.Season currentSeason, HashSet<long> passSeasonIds)
        {
            result.AlreadyRenewed = passSeasonIds.Contains(currentSeason.Id);

            result.CanRenovate = !result.AlreadyRenewed
                                 && currentSeason.PreviousSeasonId is long prevId
                                 && passSeasonIds.Contains(prevId);
        }
    }
}
