using Domain.Common;

namespace Domain.Entities;

public class Bus : BaseEntity
{
    public string CompanyName { get; private set; } = string.Empty;
    public string BusName { get; private set; } = string.Empty;
    public string BusNumber { get; private set; } = string.Empty;
    public string BusType { get; private set; } = string.Empty; // AC, Non-AC, Sleeper, etc.
    public int TotalSeats { get; private set; }

    // Navigation Properties
    public ICollection<BusSchedule> BusSchedules { get; private set; } = new List<BusSchedule>();

    // Private constructor for EF Core
    private Bus() { }

    public Bus(string companyName, string busName, string busNumber, string busType, int totalSeats)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("Company name cannot be empty", nameof(companyName));

        if (string.IsNullOrWhiteSpace(busName))
            throw new ArgumentException("Bus name cannot be empty", nameof(busName));

        if (string.IsNullOrWhiteSpace(busNumber))
            throw new ArgumentException("Bus number cannot be empty", nameof(busNumber));

        if (totalSeats <= 0)
            throw new ArgumentException("Total seats must be greater than zero", nameof(totalSeats));

        CompanyName = companyName;
        BusName = busName;
        BusNumber = busNumber;
        BusType = busType;
        TotalSeats = totalSeats;
    }

    public void UpdateDetails(string companyName, string busName, string busNumber, string busType, int totalSeats)
    {
        CompanyName = companyName;
        BusName = busName;
        BusNumber = busNumber;
        BusType = busType;
        TotalSeats = totalSeats;
        SetUpdatedAt();
    }
}
