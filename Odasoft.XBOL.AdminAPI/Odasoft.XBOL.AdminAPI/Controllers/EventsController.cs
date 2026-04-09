using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.Commons.Enums;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Response;
using Odasoft.XBOL.DTO.Results;

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
        /// <param name="sortBy">The property name by which to sort the events. Valid options: 'name', 'category', 'venue', 'createdat'.
        /// Defaults to schedule start date if null or unrecognized.</param>
        /// <param name="descending">Indicates whether the sorting should be in descending order. If true, results are sorted in descending
        /// order; otherwise, ascending order is used. If null, the default sort direction is applied.</param>
        /// <param name="page">The page number of results to retrieve. Must be a positive integer if specified. If null, the first page is
        /// returned.</param>
        /// <param name="pageSize">The number of events to include per page. Must be a positive integer if specified. If null, a default page
        /// size is used.</param>
        /// <param name="seasonId">The identifier of the season to filter events by. If null, events from all seasons are included.</param>
        /// <param name="status">The status value used to filter events (such as Active, Cancelled, etc.). If null, events of all statuses
        /// are included.</param>
        /// <param name="upcoming">Indicates whether to filter for events that have an schedule currently on sale. If null, all events are included</param>
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
            [FromQuery] bool? upcoming)
        {
            var result = await eventService.GetEventListAsync(
                venues, categories, startDate, endDate, search, sortBy, descending, page, pageSize,
                seasonId, status, upcoming);

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
        /// <param name="sortBy">The property name by which to sort the events. Valid options: 'name', 'category', 'venue'.
        /// Defaults to schedule start date if null or unrecognized.</param>
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
        /// The response includes pagination metadata and the filtered event data.</returns>
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

        /// <summary>
        /// Creates a new event using the specified request data and returns the result of the operation.
        /// </summary>
        /// <remarks>If the model state is invalid, the method returns a 400 Bad Request response. On
        /// successful creation, a 201 Created response is returned with the location of the newly created event. If
        /// the creation fails, a 422 Unprocessable Entity response is returned.</remarks>
        /// <param name="request">The request object containing the details required to create a new event. Must not be null and must satisfy
        /// model validation requirements.</param>
        /// <returns>An ActionResult containing the created EventResult if successful; otherwise, a BadRequest result if the
        /// input is invalid, or an UnprocessableEntity result if the event could not be created.</returns>
        [HttpPost]
        [EndpointName("CreateEventAsync")]
        [ProducesResponseType(typeof(EventResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<EventResult>> CreateEventAsync([FromBody] CreateEventRequest request)
        {
            var result = await eventService.CreateEventAsync(request);

            if (result != null)
            {
                return CreatedAtAction("GetEventById", new { eventId = result.Id }, result);
            }

            // TODO: Internationalization
            return UnprocessableEntity("Unable to create Event");
        }

        /// <summary>
        /// Updates the details of an existing event identified by its unique identifier.
        /// </summary>
        /// <remarks>The method validates the input model state before attempting to update the event. If
        /// the model state is invalid, a BadRequest response is returned. If the update cannot be processed, an
        /// UnprocessableEntity response is returned.</remarks>
        /// <param name="id">The unique identifier of the event to update. Must be a positive long value.</param>
        /// <param name="request">An object containing the updated event details. This parameter is required and cannot be null.</param>
        /// <returns>An IActionResult that indicates the result of the update operation. Returns NoContent if the update is
        /// successful; otherwise, returns BadRequest if the input is invalid or UnprocessableEntity if the update
        /// fails.</returns>
        [HttpPut("{id:long}")]
        [EndpointName("UpdateEventAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEventAsync([FromRoute] long id, [FromBody] UpdateEventRequest request)
        {
            var result = await eventService.UpdateEventAsync(id, request);

            if (result)
            {
                return NoContent();
            }

            // TODO: Internationalization
            return UnprocessableEntity("Unable to update Event");
        }

        /// <summary>
        /// Deletes the event identified by the specified unique identifier.
        /// </summary>
        /// <remarks>This method does not delete a event if the specified identifier does not match any
        /// existing event.</remarks>
        /// <param name="id">The unique identifier of the event to delete. Must be a valid long integer corresponding to an existing
        /// event.</param>
        /// <returns>An IActionResult that indicates the result of the delete operation. Returns 204 No Content if the deletion
        /// is successful; otherwise, returns 422 Unprocessable Entity with an error message.</returns>
        [HttpDelete("{id:long}")]
        [EndpointName("DeleteEventAsync")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DeleteEventAsync([FromRoute] long id)
        {
            var result = await eventService.DeleteEventAsync(id);

            if (result)
            {
                return NoContent();
            }

            // TODO: Internationalization
            return UnprocessableEntity("Unable to delete Event");
        }
    }
}
