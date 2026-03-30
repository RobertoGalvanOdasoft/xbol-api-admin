using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.Response;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventsController(EventService eventService) : ControllerBase
    {
        /// <summary>
        /// Retrieves a paginated list of events that match the specified filter and sorting criteria.
        /// </summary>
        /// <param name="venues">A comma-separated list of venue identifiers used to filter events by their associated venues. If null or
        /// empty, events from all venues are included.</param>
        /// <param name="categories">A comma-separated list of category identifiers used to filter events by their associated categories. If null
        /// or empty, events from all categories are included.</param>
        /// <param name="startDate">The start date and time for filtering events. Only events occurring on or after this date are included. If
        /// null, no lower date bound is applied.</param>
        /// <param name="endDate">The end date and time for filtering events. Only events occurring on or before this date are included. If
        /// null, no upper date bound is applied.</param>
        /// <param name="search">A search term used to filter events by their title or description. If null or empty, no search filtering is
        /// applied.</param>
        /// <param name="sortBy">The property name by which to sort the events. Valid options include 'date', 'name', and others supported by
        /// the API. If null or empty, a default sort order is applied.</param>
        /// <param name="descending">Indicates whether the sorting should be in descending order. If true, results are sorted in descending
        /// order; otherwise, ascending order is used. If null, the default sort direction is applied.</param>
        /// <param name="page">The page number of results to retrieve. Must be a positive integer if specified. If null, the first page is
        /// returned.</param>
        /// <param name="pageSize">The number of events to include per page. Must be a positive integer if specified. If null, a default page
        /// size is used.</param>
        /// <param name="seasonId">The identifier of the season to filter events by. If null, events from all seasons are included.</param>
        /// <param name="status">The status value used to filter events (such as Active, Cancelled, etc.). If null, events of all statuses
        /// are included.</param>
        /// <param name="onSale">Indicates whether to filter for events that have an schedule currently on sale. If null, all events are included</param>
        /// <returns>An ActionResult containing a paged response of event list items that match the specified filters and sorting
        /// options. The response includes pagination metadata and the filtered event data.</returns>
        [HttpGet]
        [EndpointName("GetEventsAsync")]
        [ProducesResponseType(typeof(PagedResponse<EventListItemDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<EventListItemDTO>>> GetEventsAsync(
            [FromQuery] string? venues,
            [FromQuery] string? categories,
            [FromQuery] DateTimeOffset? startDate,
            [FromQuery] DateTimeOffset? endDate,
            [FromQuery] string? search,
            [FromQuery] string? sortBy,
            [FromQuery] bool? descending,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            [FromQuery] long? seasonId,
            [FromQuery] EventStatus? status,
            [FromQuery] bool? onSale)
        {
            var result = await eventService.GetEventListAsync(
                venues, categories, startDate, endDate, search, sortBy, descending, page, pageSize,
                seasonId, status, onSale);

            return Ok(result);
        }


        /// <summary>
        /// Retrieves a paginated list of events that are currently available for sale that match the specified filter and sorting criteria.
        /// </summary>
        /// <param name="venues">A comma-separated list of venue identifiers used to filter events by their associated venues. If null or
        /// empty, events from all venues are included.</param>
        /// <param name="categories">A comma-separated list of category identifiers used to filter events by their associated categories. If null
        /// or empty, events from all categories are included.</param>
        /// <param name="startDate">The start date and time for filtering events. Only events occurring on or after this date are included. If
        /// null, no lower date bound is applied.</param>
        /// <param name="endDate">The end date and time for filtering events. Only events occurring on or before this date are included. If
        /// null, no upper date bound is applied.</param>
        /// <param name="search">A search term used to filter events by their title or description. If null or empty, no search filtering is
        /// applied.</param>
        /// <param name="sortBy">The property name by which to sort the events. Valid options include 'date', 'name', and others supported by
        /// the API. If null or empty, a default sort order is applied.</param>
        /// <param name="descending">Indicates whether the sorting should be in descending order. If true, results are sorted in descending
        /// order; otherwise, ascending order is used. If null, the default sort direction is applied.</param>
        /// <param name="page">The page number of results to retrieve. Must be a positive integer if specified. If null, the first page is
        /// returned.</param>
        /// <param name="pageSize">The number of events to include per page. Must be a positive integer if specified. If null, a default page
        /// size is used.</param>
        /// <returns>An ActionResult containing a paged response of event list items that are currently on sale that match
        /// the specified filter and sorting criteria. The events are filtered based on the current date and time to include
        /// only those with schedules that are actively on sale.
        /// The results are sorted by scheduled start date in ascending order. If no events are currently on sale, an empty collection is returned.
        ///The response includes pagination metadata and the filtered event data.</returns>
        [HttpGet("on-sale")]
        [EndpointName("GetEventsOnSaleAsync")]
        [ProducesResponseType(typeof(PagedResponse<EventListItemDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<EventListItemDTO>>> GetEventsOnSaleAsync(
            [FromQuery] string? venues,
            [FromQuery] string? categories,
            [FromQuery] DateTimeOffset? startDate,
            [FromQuery] DateTimeOffset? endDate,
            [FromQuery] string? search,
            [FromQuery] string? sortBy,
            [FromQuery] bool? descending,
            [FromQuery] int? page,
            [FromQuery] int? pageSize)
        {
            return await eventService.GetEventsOnSaleAsync(
                 venues, categories, startDate, endDate, search, sortBy, descending, page, pageSize);
        }

        /// <summary>
        /// Retrieves the event details for the specified event identifier.
        /// </summary>
        /// <remarks>This method asynchronously fetches event information from the event service. If the
        /// event is not found, a 404 response is returned.</remarks>
        /// <param name="eventId">The unique identifier of the event to retrieve. Must be a positive long value.</param>
        /// <returns>An ActionResult containing the EventInfoDTO representing the event details if found; otherwise, a 404 Not
        /// Found response if no event exists with the specified identifier.</returns>
        [HttpGet("{eventId:long}")]
        [EndpointName("GetEventByIdAsync")]
        [ProducesResponseType(typeof(EventInfoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventInfoDTO>> GetEventByIdAsync([FromRoute] long eventId)
        {
            var result = await eventService.GetEventByIdAsync(eventId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Retrieves the event catalog as a collection of list items.
        /// </summary>
        /// <returns>An object containing the event catalog. The collection will
        /// be empty if no items are available.</returns>
        [HttpGet("catalog")]
        [EndpointName("GetEventCatalogAsync")]
        [ProducesResponseType(typeof(List<ListItem>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ListItem>>> GetEventCatalogAsync()
        {
            var result = await eventService.GetEventCatalogAsync();
            return Ok(result);
        }
    }
}
