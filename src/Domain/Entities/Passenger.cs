using Domain.Common;

namespace Domain.Entities;

public class Passenger : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string MobileNumber { get; private set; } = string.Empty;
    public string? Email { get; private set; }

    // Navigation Properties
    public ICollection<Ticket> Tickets { get; private set; } = new List<Ticket>();

    // Private constructor for EF Core
    private Passenger() { }

    public Passenger(string name, string mobileNumber, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(mobileNumber))
            throw new ArgumentException("Mobile number cannot be empty", nameof(mobileNumber));

        Name = name;
        MobileNumber = mobileNumber;
        Email = email;
    }

    public void UpdateDetails(string name, string mobileNumber, string? email = null)
    {
        Name = name;
        MobileNumber = mobileNumber;
        Email = email;
        SetUpdatedAt();
    }
}
