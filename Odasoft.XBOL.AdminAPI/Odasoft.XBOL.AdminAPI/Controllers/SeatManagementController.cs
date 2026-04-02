using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business;
using Odasoft.XBOL.Business.Messages;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO;
using Odasoft.XBOL.DTO.Requests;
using Odasoft.XBOL.DTO.Results;
using Wolverine;

namespace Odasoft.XBOL.AdminAPI.Controllers;

[Route("api/seat-management")]
[ApiController]
public class SeatManagementController(
    SeatManagementService seatManagementService,
    ITicketingClient ticketingClient,
    IMessageBus bus) : ControllerBase
{
    /// <summary>
    /// Retrieves the seat management detail for a bookable unit identified by its Seats.io external key.
    /// </summary>
    /// <remarks>Resolves the external key against event schedules first, then seasons.
    /// Returns the data needed to render the seat selector/availability page.</remarks>
    /// <param name="externalKey">The Seats.io external key identifying the event schedule or season.</param>
    /// <returns>The seat management detail if found; otherwise, a 404 Not Found response.</returns>
    [HttpGet("{externalKey}")]
    [EndpointName("GetSeatManagementDetailAsync")]
    [ProducesResponseType(typeof(SeatManagementDetailDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SeatManagementDetailDTO>> GetSeatManagementDetailAsync(
        [FromRoute] string externalKey)
    {
        var result = await seatManagementService.GetDetailAsync(externalKey);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Blocks the specified seats, setting reason and color metadata in Seats.io
    /// and marking them as not for sale in the local database.
    /// </summary>
    /// <param name="externalKey">The Seats.io external key identifying the event schedule or season.</param>
    /// <param name="request">The seat keys to block along with the reason and color for visual identification.</param>
    /// <returns>No content on success; 404 if the external key does not resolve.</returns>
    [HttpPost("{externalKey}/block")]
    [EndpointName("BlockSeatsAsync")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BlockSeatsAsync(
        [FromRoute] string externalKey,
        [FromBody] BlockSeatsRequest request)
    {
        var found = await seatManagementService.BlockSeatsAsync(externalKey, request);
        return found ? NoContent() : NotFound();
    }

    /// <summary>
    /// Unblocks the specified seats, making them available for sale again
    /// and clearing their metadata from Seats.io.
    /// </summary>
    /// <param name="externalKey">The Seats.io external key identifying the event schedule or season.</param>
    /// <param name="request">The seat keys to unblock.</param>
    /// <returns>No content on success; 404 if the external key does not resolve.</returns>
    [HttpPost("{externalKey}/unblock")]
    [EndpointName("UnblockSeatsAsync")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnblockSeatsAsync(
        [FromRoute] string externalKey,
        [FromBody] UnblockSeatsRequest request)
    {
        var found = await seatManagementService.UnblockSeatsAsync(externalKey, request);
        return found ? NoContent() : NotFound();
    }

    /// <summary>
    /// Updates the reason and color metadata of already-blocked seats without changing their for-sale status.
    /// </summary>
    /// <param name="externalKey">The Seats.io external key identifying the event schedule or season.</param>
    /// <param name="request">The seat keys along with the updated reason and color.</param>
    /// <returns>No content on success; 404 if the external key does not resolve.</returns>
    [HttpPut("{externalKey}/block")]
    [EndpointName("EditBlockedSeatsAsync")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditBlockedSeatsAsync(
        [FromRoute] string externalKey,
        [FromBody] BlockSeatsRequest request)
    {
        var found = await seatManagementService.EditBlockedSeatsAsync(externalKey, request);
        return found ? NoContent() : NotFound();
    }

    /// <summary>
    /// Holds the specified seats by creating a temporary hold token.
    /// </summary>
    /// <param name="externalKey">The Seats.io external key identifying the event schedule or season.</param>
    /// <param name="request">The seat labels to hold.</param>
    /// <returns>A hold token containing the token string, expiry, and workspace key.</returns>
    [HttpPost("{externalKey}/hold")]
    [EndpointName("HoldSeatsByKeyAsync")]
    [ProducesResponseType(typeof(HoldToken), StatusCodes.Status200OK)]
    public async Task<ActionResult<HoldToken>> HoldSeatsAsync(
        [FromRoute] string externalKey,
        [FromBody] HoldSeatsBody request)
    {
        var token = await ticketingClient.HoldSeatsActionAsync(new HoldSeatsActionRequest
        {
            EventKey = externalKey,
            Seats = request.Seats
        });
        return Ok(token);
    }

    /// <summary>
    /// Releases the specified seats, resetting their status to free.
    /// </summary>
    /// <param name="externalKey">The Seats.io external key identifying the event schedule or season.</param>
    /// <param name="request">The seat labels to release, with an optional hold token to scope the release.</param>
    /// <returns>A list of released seat keys.</returns>
    [HttpPost("{externalKey}/release")]
    [EndpointName("ReleaseSeatsByKeyAsync")]
    [ProducesResponseType(typeof(ICollection<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ICollection<string>>> ReleaseSeatsAsync(
        [FromRoute] string externalKey,
        [FromBody] ReleaseSeatsBody request)
    {
        var result = await ticketingClient.ReleaseSeatsActionAsync(new ReleaseSeatsByKeyRequest
        {
            EventKey = externalKey,
            Seats = request.Seats,
            HoldToken = request.HoldToken,
            KeepExtraData = false
        });
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the hold token details for the specified token string.
    /// </summary>
    /// <param name="holdToken">The hold token string to look up.</param>
    /// <returns>The hold token details including expiry information.</returns>
    [HttpGet("hold/{holdToken}")]
    [EndpointName("GetHoldTokenByKeyAsync")]
    [ProducesResponseType(typeof(HoldToken), StatusCodes.Status200OK)]
    public async Task<ActionResult<HoldToken>> GetHoldTokenAsync(
        [FromRoute] string holdToken)
    {
        var result = await ticketingClient.GetHoldTokenActionAsync(holdToken);
        return Ok(result);
    }

    /// <summary>
    /// Expires the specified hold token, releasing all seats held by it.
    /// </summary>
    /// <param name="holdToken">The hold token to expire.</param>
    /// <returns>The expired hold token details.</returns>
    [HttpDelete("hold/{holdToken}")]
    [EndpointName("ExpireHoldTokenByKeyAsync")]
    [ProducesResponseType(typeof(HoldToken), StatusCodes.Status200OK)]
    public async Task<ActionResult<HoldToken>> ExpireHoldTokenAsync(
        [FromRoute] string holdToken)
    {
        var result = await ticketingClient.ExpireHoldTokenActionAsync(holdToken);
        return Ok(result);
    }

    /// <summary>
    /// Books the specified seats, optionally consuming a hold token,
    /// and creates the corresponding Order in the database.
    /// </summary>
    /// <param name="externalKey">The Seats.io external key identifying the event schedule or season.</param>
    /// <param name="request">The seats with prices, optional hold token, and booking details.</param>
    /// <returns>A booking result containing the order ID, reference, and booked seat keys.</returns>
    [HttpPost("{externalKey}/book")]
    [EndpointName("BookSeatsByKeyAsync")]
    [ProducesResponseType(typeof(BookingResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<BookingResult>> BookSeatsAsync(
        [FromRoute] string externalKey,
        [FromBody] BookSeatsBody request)
    {
        var resolved = await seatManagementService.ResolveExternalKeyAsync(externalKey);
        if (resolved is null) return NotFound();

        BookingResult? result;

        if (resolved.Type == Commons.Enums.BookableUnitType.Schedule)
        {
            result = await bus.InvokeAsync<BookingResult>(new CreateEventBookingCommand(new EventBookingRequest
            {
                EventKey = externalKey,
                Seats = request.Seats,
                HoldToken = request.HoldToken,
                EventScheduleId = request.EventScheduleId,
                TicketType = request.TicketType,
                ClientContact = request.ClientContact,
                PaymentInfoRequest = request.PaymentInfoRequest,
                Localizer = request.Localizer
            }));
        }
        else
        {
            result = await bus.InvokeAsync<BookingResult>(new CreateSeasonBookingCommand(new SeasonBookingRequest
            {
                SeasonKey = externalKey,
                Seats = request.Seats,
                HoldToken = request.HoldToken,
                EventScheduleId = request.EventScheduleId,
                TicketType = request.TicketType,
                ClientContact = request.ClientContact,
                PaymentInfoRequest = request.PaymentInfoRequest,
                Localizer = request.Localizer,
                RefereceOrderId = request.ReferenceOrderId
            }));
        }

        if (result is null)
        {
            return UnprocessableEntity("Booking failed. Please check the request details and try again.");
        }

        return Ok(result);
    }
}

public class HoldSeatsBody
{
    public required ICollection<string> Seats { get; set; }
}

public class ReleaseSeatsBody
{
    public required ICollection<string> Seats { get; set; }
    public string? HoldToken { get; set; }
}

public class BookSeatsBody
{
    public required IDictionary<string, decimal> Seats { get; set; }
    public string? HoldToken { get; set; }
    public long? EventScheduleId { get; set; }
    public required ItemType TicketType { get; set; }
    public required ClientInfoRequest ClientContact { get; set; }
    public required PaymentInfoRequest PaymentInfoRequest { get; set; }
    public string? Localizer { get; set; }
    public long? ReferenceOrderId { get; set; }
}
