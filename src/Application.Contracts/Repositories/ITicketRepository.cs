using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface ITicketRepository : IRepository<Ticket>
{
    Task<Ticket?> GetByBookingReferenceAsync(string bookingReference);
    Task<List<Ticket>> GetTicketsByPassengerAsync(Guid passengerId);
}
