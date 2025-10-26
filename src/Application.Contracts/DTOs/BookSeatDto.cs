namespace Application.Contracts.DTOs;

public class BookSeatInputDto
{
    public Guid BusScheduleId { get; set; }
    public Guid SeatId { get; set; }
    public string PassengerName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string BoardingPoint { get; set; } = string.Empty;
    public string DroppingPoint { get; set; } = string.Empty;
}

public class BookSeatResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? BookingReference { get; set; }
    public Guid? TicketId { get; set; }
    public decimal? TotalAmount { get; set; }
}
