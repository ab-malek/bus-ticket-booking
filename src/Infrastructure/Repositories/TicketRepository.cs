using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    public TicketRepository(BusReservationDbContext context) : base(context)
    {
    }

    public async Task<Ticket?> GetByBookingReferenceAsync(string bookingReference)
    {
        return await _context.Tickets
            .Include(t => t.Passenger)
            .Include(t => t.Seat)
            .ThenInclude(s => s.BusSchedule)
            .ThenInclude(bs => bs.Bus)
            .FirstOrDefaultAsync(t => t.BookingReference == bookingReference);
    }

    public async Task<List<Ticket>> GetTicketsByPassengerAsync(Guid passengerId)
    {
        return await _context.Tickets
            .Include(t => t.Seat)
            .Where(t => t.PassengerId == passengerId)
            .ToListAsync();
    }
}
