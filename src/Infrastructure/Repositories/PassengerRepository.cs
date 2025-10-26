using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PassengerRepository : Repository<Passenger>, IPassengerRepository
{
    public PassengerRepository(BusReservationDbContext context) : base(context)
    {
    }

    public async Task<Passenger?> FindByMobileNumberAsync(string mobileNumber)
    {
        return await _context.Passengers
            .FirstOrDefaultAsync(p => p.MobileNumber == mobileNumber);
    }
}
