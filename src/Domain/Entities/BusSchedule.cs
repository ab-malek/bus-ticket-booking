using Domain.Common;

namespace Domain.Entities;

public class BusSchedule : BaseEntity
{
    public Guid BusId { get; private set; }
    public Guid RouteId { get; private set; }
    public DateTime JourneyDate { get; private set; }
    public TimeSpan DepartureTime { get; private set; }
    public TimeSpan ArrivalTime { get; private set; }
    public decimal Fare { get; private set; }
    public string BoardingPoint { get; private set; } = string.Empty;
    public string DroppingPoint { get; private set; } = string.Empty;

    // Navigation Properties
    public Bus Bus { get; private set; } = null!;
    public Route Route { get; private set; } = null!;
    public ICollection<Seat> Seats { get; private set; } = new List<Seat>();

    // Private constructor for EF Core
    private BusSchedule() { }

    public BusSchedule(
        Guid busId,
        Guid routeId,
        DateTime journeyDate,
        TimeSpan departureTime,
        TimeSpan arrivalTime,
        decimal fare,
        string boardingPoint,
        string droppingPoint)
    {
        if (busId == Guid.Empty)
            throw new ArgumentException("Bus ID cannot be empty", nameof(busId));

        if (routeId == Guid.Empty)
            throw new ArgumentException("Route ID cannot be empty", nameof(routeId));

        if (fare <= 0)
            throw new ArgumentException("Fare must be greater than zero", nameof(fare));

        BusId = busId;
        RouteId = routeId;
        JourneyDate = journeyDate.Date;
        DepartureTime = departureTime;
        ArrivalTime = arrivalTime;
        Fare = fare;
        BoardingPoint = boardingPoint;
        DroppingPoint = droppingPoint;
    }

    public void UpdateSchedule(DateTime journeyDate, TimeSpan departureTime, TimeSpan arrivalTime, decimal fare)
    {
        JourneyDate = journeyDate.Date;
        DepartureTime = departureTime;
        ArrivalTime = arrivalTime;
        Fare = fare;
        SetUpdatedAt();
    }

    public void InitializeSeats(int totalSeats)
    {
        if (Seats.Any())
            throw new InvalidOperationException("Seats have already been initialized");

        for (int i = 1; i <= totalSeats; i++)
        {
            var seat = new Seat(Id, i.ToString(), CalculateRow(i));
            ((List<Seat>)Seats).Add(seat);
        }
    }

    private static string CalculateRow(int seatNumber)
    {
        // Simple row calculation: 4 seats per row (A, B, C, D)
        int rowNumber = (seatNumber - 1) / 4 + 1;
        return rowNumber.ToString();
    }
}
