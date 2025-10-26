using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface ISeatRepository : IRepository<Seat>
{
    Task<Seat?> GetWithDetailsAsync(Guid seatId);
    Task<List<Seat>> GetSeatsByBusScheduleAsync(Guid busScheduleId);
}
