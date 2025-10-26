using Domain.Enums;

namespace Application.Contracts.DTOs;

public class SeatDto
{
    public Guid SeatId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public string Row { get; set; } = string.Empty;
    public SeatStatus Status { get; set; }
}

public class SeatPlanDto
{
    public Guid BusScheduleId { get; set; }
    public string BusName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public DateTime JourneyDate { get; set; }
    public TimeSpan DepartureTime { get; set; }
    public TimeSpan ArrivalTime { get; set; }
    public decimal Fare { get; set; }
    public string BoardingPoint { get; set; } = string.Empty;
    public string DroppingPoint { get; set; } = string.Empty;
    public List<SeatDto> Seats { get; set; } = new();
}
