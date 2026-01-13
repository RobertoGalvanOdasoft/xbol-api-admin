using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly EventService _eventService;

        public EventsController(EventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Retrieves the details of a specific event by its unique identifier.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event to retrieve. Must be a valid event ID.</param>
        /// <returns>An <see cref="ActionResult{Event}"/> containing the event details if found; otherwise, a response indicating
        /// that the event does not exist.</returns>
        [HttpGet("{eventId}")]
        public async Task<ActionResult<Event>> GetEventDetails([FromRoute] long eventId)
        {
            var eventDetails = await _eventService.GetEventByIdAsync(eventId);

            // TODO: Set the proper Response DTO
            return Ok(eventDetails);
        }
    }
}
