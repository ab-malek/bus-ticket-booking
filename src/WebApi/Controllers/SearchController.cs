using Application.Contracts.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(ISearchService searchService, ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    /// <summary>
    /// Search for available buses between two cities on a specific date
    /// </summary>
    /// <param name="from">Departure city</param>
    /// <param name="to">Arrival city</param>
    /// <param name="journeyDate">Journey date</param>
    /// <returns>List of available buses</returns>
    [HttpGet("buses")]
    public async Task<ActionResult<List<AvailableBusDto>>> SearchBuses(
        [FromQuery] string from,
        [FromQuery] string to,
        [FromQuery] DateTime journeyDate)
    {
        try
        {
            _logger.LogInformation("Searching buses from {From} to {To} on {Date}", from, to, journeyDate);

            var buses = await _searchService.SearchAvailableBusesAsync(from, to, journeyDate);

            _logger.LogInformation("Found {Count} buses", buses.Count);
            return Ok(buses);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid search parameters");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for buses");
            return StatusCode(500, new { message = "An error occurred while searching for buses" });
        }
    }

    /// <summary>
    /// Get bus details by schedule ID
    /// </summary>
    /// <param name="id">Bus schedule ID (GUID)</param>
    /// <returns>Bus details</returns>
    [HttpGet("buses/{id}")]
    public async Task<ActionResult<AvailableBusDto>> GetBusById([FromRoute] Guid id)
    {
        try
        {
            _logger.LogInformation("Getting bus details for schedule ID: {Id}", id);

            var bus = await _searchService.GetBusByScheduleIdAsync(id);

            if (bus == null)
            {
                _logger.LogWarning("Bus not found with schedule ID: {Id}", id);
                return NotFound(new { message = "Bus not found" });
            }

            _logger.LogInformation("Found bus: {BusNumber}", bus.BusNumber);
            return Ok(bus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bus details");
            return StatusCode(500, new { message = "An error occurred while retrieving bus details" });
        }
    }
}
