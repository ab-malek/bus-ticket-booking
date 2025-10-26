using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Seat : BaseEntity
{
    public Guid BusScheduleId { get; private set; }
    public string SeatNumber { get; private set; } = string.Empty;
    public string Row { get; private set; } = string.Empty;
    public SeatStatus Status { get; private set; }

    // Navigation Properties
    public BusSchedule BusSchedule { get; private set; } = null!;
    public Ticket? Ticket { get; private set; }

    // Private constructor for EF Core
    private Seat() { }

    public Seat(Guid busScheduleId, string seatNumber, string row)
    {
        if (busScheduleId == Guid.Empty)
            throw new ArgumentException("Bus schedule ID cannot be empty", nameof(busScheduleId));

        if (string.IsNullOrWhiteSpace(seatNumber))
            throw new ArgumentException("Seat number cannot be empty", nameof(seatNumber));

        BusScheduleId = busScheduleId;
        SeatNumber = seatNumber;
        Row = row;
        Status = SeatStatus.Available;
    }

    public void Book()
    {
        if (Status != SeatStatus.Available)
            throw new InvalidOperationException($"Cannot book seat {SeatNumber}. Current status: {Status}");

        Status = SeatStatus.Booked;
        SetUpdatedAt();
    }

    public void MarkAsSold()
    {
        if (Status != SeatStatus.Booked)
            throw new InvalidOperationException($"Cannot mark seat {SeatNumber} as sold. Current status: {Status}");

        Status = SeatStatus.Sold;
        SetUpdatedAt();
    }

    public void Release()
    {
        if (Status == SeatStatus.Sold)
            throw new InvalidOperationException($"Cannot release sold seat {SeatNumber}");

        Status = SeatStatus.Available;
        SetUpdatedAt();
    }

    public bool IsAvailable() => Status == SeatStatus.Available;
}
