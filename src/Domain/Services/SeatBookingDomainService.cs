using Domain.Entities;
using Domain.Enums;

namespace Domain.Services;

public interface ISeatBookingDomainService
{
    Task<Ticket> BookSeatAsync(Seat seat, Passenger passenger, string boardingPoint, string droppingPoint, decimal fare);
    bool CanBookSeat(Seat seat);
}

public class SeatBookingDomainService : ISeatBookingDomainService
{
    public async Task<Ticket> BookSeatAsync(
        Seat seat,
        Passenger passenger,
        string boardingPoint,
        string droppingPoint,
        decimal fare)
    {
        if (!CanBookSeat(seat))
        {
            throw new InvalidOperationException(
                $"Seat {seat.SeatNumber} cannot be booked. Current status: {seat.Status}");
        }

        // Book the seat
        seat.Book();

        // Create ticket
        var ticket = new Ticket(
            passenger.Id,
            seat.Id,
            boardingPoint,
            droppingPoint,
            fare);

        return await Task.FromResult(ticket);
    }

    public bool CanBookSeat(Seat seat)
    {
        return seat.Status == SeatStatus.Available;
    }
}
