using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.Response;

namespace Odasoft.XBOL.Business.Services
{
    public class EventService
    {
        private readonly EventRepository _eventRepository;

        public EventService(EventRepository eventRepository)
        {
            _eventRepository = eventRepository;
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
            EventStatus? status = null)
        {
            return await _eventRepository.GetEventListAsync(
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
                status);
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
