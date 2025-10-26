using Domain.Common;

namespace Domain.Entities;

public class Route : BaseEntity
{
    public string FromCity { get; private set; } = string.Empty;
    public string ToCity { get; private set; } = string.Empty;
    public decimal Distance { get; private set; }
    public TimeSpan EstimatedDuration { get; private set; }

    // Navigation Properties
    public ICollection<BusSchedule> BusSchedules { get; private set; } = new List<BusSchedule>();

    // Private constructor for EF Core
    private Route() { }

    public Route(string fromCity, string toCity, decimal distance, TimeSpan estimatedDuration)
    {
        if (string.IsNullOrWhiteSpace(fromCity))
            throw new ArgumentException("From city cannot be empty", nameof(fromCity));

        if (string.IsNullOrWhiteSpace(toCity))
            throw new ArgumentException("To city cannot be empty", nameof(toCity));

        if (distance <= 0)
            throw new ArgumentException("Distance must be greater than zero", nameof(distance));

        if (estimatedDuration <= TimeSpan.Zero)
            throw new ArgumentException("Estimated duration must be greater than zero", nameof(estimatedDuration));

        FromCity = fromCity;
        ToCity = toCity;
        Distance = distance;
        EstimatedDuration = estimatedDuration;
    }

    public void UpdateDetails(string fromCity, string toCity, decimal distance, TimeSpan estimatedDuration)
    {
        FromCity = fromCity;
        ToCity = toCity;
        Distance = distance;
        EstimatedDuration = estimatedDuration;
        SetUpdatedAt();
    }
}
