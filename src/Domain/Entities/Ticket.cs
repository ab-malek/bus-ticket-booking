using Domain.Common;

namespace Domain.Entities;

public class Ticket : BaseEntity
{
    public Guid PassengerId { get; private set; }
    public Guid SeatId { get; private set; }
    public string BoardingPoint { get; private set; } = string.Empty;
    public string DroppingPoint { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public string BookingReference { get; private set; } = string.Empty;
    public bool IsConfirmed { get; private set; }

    // Navigation Properties
    public Passenger Passenger { get; private set; } = null!;
    public Seat Seat { get; private set; } = null!;

    // Private constructor for EF Core
    private Ticket() { }

    public Ticket(
        Guid passengerId,
        Guid seatId,
        string boardingPoint,
        string droppingPoint,
        decimal totalAmount)
    {
        if (passengerId == Guid.Empty)
            throw new ArgumentException("Passenger ID cannot be empty", nameof(passengerId));

        if (seatId == Guid.Empty)
            throw new ArgumentException("Seat ID cannot be empty", nameof(seatId));

        if (totalAmount <= 0)
            throw new ArgumentException("Total amount must be greater than zero", nameof(totalAmount));

        PassengerId = passengerId;
        SeatId = seatId;
        BoardingPoint = boardingPoint;
        DroppingPoint = droppingPoint;
        TotalAmount = totalAmount;
        BookingReference = GenerateBookingReference();
        IsConfirmed = false;
    }

    public void Confirm()
    {
        if (IsConfirmed)
            throw new InvalidOperationException("Ticket is already confirmed");

        IsConfirmed = true;
        SetUpdatedAt();
    }

    public void Cancel()
    {
        if (!IsConfirmed)
            throw new InvalidOperationException("Cannot cancel unconfirmed ticket");

        IsConfirmed = false;
        SetUpdatedAt();
    }

    private static string GenerateBookingReference()
    {
        return $"BKG{DateTime.UtcNow:yyyyMMddHHmmss}{Guid.NewGuid().ToString()[..6].ToUpper()}";
    }
}
