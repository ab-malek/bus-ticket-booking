using Application.Contracts.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingController> _logger;

    public BookingController(IBookingService bookingService, ILogger<BookingController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>
    /// Get seat plan for a specific bus schedule
    /// </summary>
    /// <param name="busScheduleId">Bus schedule ID</param>
    /// <returns>Seat plan with all seats and their statuses</returns>
    [HttpGet("seat-plan/{busScheduleId}")]
    public async Task<ActionResult<SeatPlanDto>> GetSeatPlan(Guid busScheduleId)
    {
        try
        {
            _logger.LogInformation("Retrieving seat plan for bus schedule {BusScheduleId}", busScheduleId);

            var seatPlan = await _bookingService.GetSeatPlanAsync(busScheduleId);

            return Ok(seatPlan);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Bus schedule not found: {BusScheduleId}", busScheduleId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving seat plan");
            return StatusCode(500, new { message = "An error occurred while retrieving seat plan" });
        }
    }

    /// <summary>
    /// Book a seat for a passenger
    /// </summary>
    /// <param name="input">Booking details</param>
    /// <returns>Booking confirmation message</returns>
    [HttpPost("book-seat")]
    public async Task<ActionResult> BookSeat([FromBody] BookSeatInputDto input)
    {
        try
        {
            _logger.LogInformation("Booking seat {SeatId} for passenger {Name}",
                input.SeatId, input.PassengerName);

            var result = await _bookingService.BookSeatAsync(input);

            if (!result.Success)
            {
                _logger.LogWarning("Booking failed: {Message}", result.Message);
                return BadRequest(new { message = result.Message });
            }

            _logger.LogInformation("Seat booked successfully. Booking reference: {BookingRef}",
                result.BookingReference);

            return Ok(new { message = "Booking confirmed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking seat");
            return StatusCode(500, new { message = "An error occurred while booking the seat" });
        }
    }
}
