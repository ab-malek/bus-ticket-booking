namespace Application.Contracts.DTOs;

public class AvailableBusDto
{
    public Guid BusScheduleId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;

    // Alias for frontend compatibility
    public string BusNumber => BusName;

    public string BusType { get; set; } = string.Empty;
    public DateTime JourneyDate { get; set; }
    public TimeSpan DepartureTime { get; set; }
    public TimeSpan ArrivalTime { get; set; }
    public int TotalSeats { get; set; }
    public int BookedSeats { get; set; }
    public int SeatsLeft { get; set; }

    // Alias for frontend compatibility
    public int AvailableSeats => SeatsLeft;

    public decimal Price { get; set; }

    // Alias for frontend compatibility
    public decimal Fare => Price;

    public string BoardingPoint { get; set; } = string.Empty;
    public string DroppingPoint { get; set; } = string.Empty;
}
