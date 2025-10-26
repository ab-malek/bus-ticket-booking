using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IPassengerRepository : IRepository<Passenger>
{
    Task<Passenger?> FindByMobileNumberAsync(string mobileNumber);
}
