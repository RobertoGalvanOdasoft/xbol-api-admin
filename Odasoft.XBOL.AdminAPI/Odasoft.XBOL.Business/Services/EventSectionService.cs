using Odasoft.XBOL.Data.Repositories;
using Odasoft.XBOL.DTO;

namespace Odasoft.XBOL.Business.Services
{
    public class EventSectionService(EventSectionRepository _eventSectionRepository)
    {
        public async Task<List<ZonePriceDTO>> GetZonePricesByEventIdAsync(long eventId)
        {
            var zonePrices = await _eventSectionRepository.GetEventZonePricesAsync(eventId);
            return zonePrices;
        }
    }
}
