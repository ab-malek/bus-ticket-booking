using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IBusScheduleRepository : IRepository<BusSchedule>
{
    Task<List<BusSchedule>> SearchAvailableBusesAsync(string fromCity, string toCity, DateTime journeyDate);
    Task<BusSchedule?> GetWithSeatsAsync(Guid busScheduleId);
    Task<BusSchedule?> GetWithDetailsAsync(Guid busScheduleId);
}
