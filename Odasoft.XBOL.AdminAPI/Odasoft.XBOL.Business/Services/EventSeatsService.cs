using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO;

namespace Odasoft.XBOL.Business.Services
{
    public class EventSeatsService
    {
        private readonly EventSeatRepository _eventSeatRepository;

        public EventSeatsService(EventSeatRepository eventSeatRepository)
        {
            _eventSeatRepository = eventSeatRepository;
        }

        public async Task<IList<SeatPriceDTO>> GetSeatPricesByEventIdAsync(long eventId)
        {
            var seatPrices = await _eventSeatRepository.GetEventSeatPricesAsync(eventId);

            return seatPrices;
        }
    }
}
