using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.Data.Repositories.Season;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.Response;

namespace Odasoft.XBOL.Business.Services
{
    public class EventService
    {
        private readonly EventRepository _eventRepository;
        private readonly SeasonRepository _seasonRepository;

        public EventService(EventRepository eventRepository, SeasonRepository seasonRepository)
        {
            _eventRepository = eventRepository;
            _seasonRepository = seasonRepository;
        }

        public async Task<PagedResponse<EventListItemDTO>> GetEventListAsync(
            string? venues,
            string? categories,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            string? search,
            string? sortBy,
            bool? descending,
            int? page,
            int? pageSize,
            long? seasonId = null,
            EventStatus? status = null,
            bool? onSale = null)
        {

            var events = await _eventRepository.GetEventListAsync(
                venues,
                categories,
                startDate,
                endDate,
                search,
                sortBy,
                descending ?? false,
                page ?? 1,
                pageSize ?? 10,
                seasonId,
                status,
                onSale);

            // TODO: Do a proper method to get all types of event
            var seasons = _seasonRepository.Get(x => x.Status == SeasonStatus.Published);


            if (onSale.HasValue)
            {
                if (onSale.Value)
                {
                    seasons = seasons.Where(x => x.OnSaleDate <= DateTimeOffset.UtcNow && x.OffSaleDate >= DateTimeOffset.UtcNow);
                }
                else
                {
                    seasons = seasons.Where(x => x.OffSaleDate <= DateTimeOffset.UtcNow);
                }
            }

            var resultSeasons = seasons.Select(x => new EventListItemDTO
            {
                Id = x.Id,
                Name = x.Name,
                ScheduledStartDate = x.StartDate,
                Category = "Season",
                VenueMapId = 0, // Seasons may not have a venue, set to 0 or handle accordingly
                VenueName = null,
                ExternalEventKey = x.ExternalSeasonKey,
                AvailableSeats = 0, // Seasons may not have seat information, set to 0 or handle accordingly
                TotalSeats = 0, // Seasons may not have seat information, set to 0 or handle accordingly
                PosterImageUrl = x.PosterImageUrl,
                IsSeason = true
            });

            events.Items.AddRange(resultSeasons);

            return events;
        }

        public async Task<EventInfoDTO?> GetEventByIdAsync(long eventId)
        {
            var result = await _eventRepository.GetEventByIdAsync(eventId);

            // TODO: Handle null result (e.g., throw exception or return a default value)
            return result;
        }

        public async Task<IList<ListItem>> GetEventCatalogAsync()
        {
            return await _eventRepository
                            .Get()
                            .AsNoTracking()
                            .Select(x => new ListItem
                            {
                                Id = x.Id,
                                Name = x.Name
                            }).ToListAsync();
        }
    }
}
