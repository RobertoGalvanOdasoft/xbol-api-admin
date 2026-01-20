using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Business.Services
{
    public class EventService
    {
        private readonly EventRepository _eventRepository;

        public EventService(EventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<Event?> GetEventByIdAsync(long eventId)
        {
            // TODO: Set the proper DTO to return only necessary fields
            var result = await _eventRepository.GetByIdAsync(eventId);

            // TODO: Handle null result (e.g., throw exception or return a default value)
            return result;
        }
    }
}
