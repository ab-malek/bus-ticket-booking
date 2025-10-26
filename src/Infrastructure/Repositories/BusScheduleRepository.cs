using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BusScheduleRepository : Repository<BusSchedule>, IBusScheduleRepository
{
    public BusScheduleRepository(BusReservationDbContext context) : base(context)
    {
    }

    public async Task<List<BusSchedule>> SearchAvailableBusesAsync(string fromCity, string toCity, DateTime journeyDate)
    {
        return await _context.BusSchedules
            .Include(bs => bs.Bus)
            .Include(bs => bs.Route)
            .Include(bs => bs.Seats)
            .Where(bs => bs.Route.FromCity.ToLower() == fromCity.ToLower() &&
                        bs.Route.ToCity.ToLower() == toCity.ToLower() &&
                        bs.JourneyDate.Date == journeyDate.Date)
            .ToListAsync();
    }

    public async Task<BusSchedule?> GetWithSeatsAsync(Guid busScheduleId)
    {
        return await _context.BusSchedules
            .Include(bs => bs.Bus)
            .Include(bs => bs.Seats)
            .FirstOrDefaultAsync(bs => bs.Id == busScheduleId);
    }

    public async Task<BusSchedule?> GetWithDetailsAsync(Guid busScheduleId)
    {
        return await _context.BusSchedules
            .Include(bs => bs.Bus)
            .Include(bs => bs.Route)
            .Include(bs => bs.Seats)
            .FirstOrDefaultAsync(bs => bs.Id == busScheduleId);
    }
}
