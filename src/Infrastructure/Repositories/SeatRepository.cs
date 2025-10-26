using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SeatRepository : Repository<Seat>, ISeatRepository
{
    public SeatRepository(BusReservationDbContext context) : base(context)
    {
    }

    public async Task<Seat?> GetWithDetailsAsync(Guid seatId)
    {
        return await _context.Seats
            .Include(s => s.BusSchedule)
            .ThenInclude(bs => bs.Bus)
            .FirstOrDefaultAsync(s => s.Id == seatId);
    }

    public async Task<List<Seat>> GetSeatsByBusScheduleAsync(Guid busScheduleId)
    {
        return await _context.Seats
            .Where(s => s.BusScheduleId == busScheduleId)
            .ToListAsync();
    }
}
