using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Tests;

public class BusScheduleTests
{
    [Fact]
    public void BusSchedule_Creation_WithValidData_ShouldSucceed()
    {
        // Arrange
        var busId = Guid.NewGuid();
        var routeId = Guid.NewGuid();
        var journeyDate = DateTime.Today.AddDays(1);
        var departureTime = TimeSpan.FromHours(8);
        var arrivalTime = TimeSpan.FromHours(12);
        var fare = 45m;

        // Act
        var schedule = new BusSchedule(
            busId, routeId, journeyDate,
            departureTime, arrivalTime, fare,
            "New York", "Boston"
        );

        // Assert
        schedule.BusId.Should().Be(busId);
        schedule.RouteId.Should().Be(routeId);
        schedule.JourneyDate.Should().Be(journeyDate.Date);
        schedule.DepartureTime.Should().Be(departureTime);
        schedule.ArrivalTime.Should().Be(arrivalTime);
        schedule.Fare.Should().Be(fare);
        schedule.BoardingPoint.Should().Be("New York");
        schedule.DroppingPoint.Should().Be("Boston");
    }

    [Fact]
    public void BusSchedule_Creation_WithEmptyBusId_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new BusSchedule(
            Guid.Empty, Guid.NewGuid(), DateTime.Today,
            TimeSpan.FromHours(8), TimeSpan.FromHours(12), 45m,
            "NY", "Boston"
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Bus ID cannot be empty*");
    }

    [Fact]
    public void BusSchedule_Creation_WithZeroFare_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new BusSchedule(
            Guid.NewGuid(), Guid.NewGuid(), DateTime.Today,
            TimeSpan.FromHours(8), TimeSpan.FromHours(12), 0m,
            "NY", "Boston"
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Fare must be greater than zero*");
    }

    [Fact]
    public void BusSchedule_Creation_WithNegativeFare_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new BusSchedule(
            Guid.NewGuid(), Guid.NewGuid(), DateTime.Today,
            TimeSpan.FromHours(8), TimeSpan.FromHours(12), -10m,
            "NY", "Boston"
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Fare must be greater than zero*");
    }

    [Fact]
    public void InitializeSeats_WithValidCount_ShouldCreateSeats()
    {
        // Arrange
        var schedule = new BusSchedule(
            Guid.NewGuid(), Guid.NewGuid(), DateTime.Today,
            TimeSpan.FromHours(8), TimeSpan.FromHours(12), 45m,
            "NY", "Boston"
        );

        // Act
        schedule.InitializeSeats(40);

        // Assert
        schedule.Seats.Should().HaveCount(40);
        schedule.Seats.Should().AllSatisfy(s =>
        {
            s.SeatNumber.Should().NotBeNullOrEmpty();
            s.Row.Should().NotBeNullOrEmpty();
            s.Status.Should().Be(Domain.Enums.SeatStatus.Available);
        });
    }

    [Fact]
    public void InitializeSeats_ShouldCreateSeatsWithSequentialNumbers()
    {
        // Arrange
        var schedule = new BusSchedule(
            Guid.NewGuid(), Guid.NewGuid(), DateTime.Today,
            TimeSpan.FromHours(8), TimeSpan.FromHours(12), 45m,
            "NY", "Boston"
        );

        // Act
        schedule.InitializeSeats(10);

        // Assert
        var seatNumbers = schedule.Seats.Select(s => s.SeatNumber).ToList();
        seatNumbers.Should().Contain("1");
        seatNumbers.Should().Contain("5");
        seatNumbers.Should().Contain("10");
    }

    [Fact]
    public void InitializeSeats_WhenAlreadyInitialized_ShouldThrowException()
    {
        // Arrange
        var schedule = new BusSchedule(
            Guid.NewGuid(), Guid.NewGuid(), DateTime.Today,
            TimeSpan.FromHours(8), TimeSpan.FromHours(12), 45m,
            "NY", "Boston"
        );
        schedule.InitializeSeats(40);

        // Act
        var act = () => schedule.InitializeSeats(40);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Seats have already been initialized*");
    }

    [Fact]
    public void UpdateSchedule_WithValidData_ShouldUpdateProperties()
    {
        // Arrange
        var schedule = new BusSchedule(
            Guid.NewGuid(), Guid.NewGuid(), DateTime.Today,
            TimeSpan.FromHours(8), TimeSpan.FromHours(12), 45m,
            "NY", "Boston"
        );

        var newJourneyDate = DateTime.Today.AddDays(2);
        var newDepartureTime = TimeSpan.FromHours(10);
        var newArrivalTime = TimeSpan.FromHours(14);
        var newFare = 55m;

        // Act
        schedule.UpdateSchedule(newJourneyDate, newDepartureTime, newArrivalTime, newFare);

        // Assert
        schedule.JourneyDate.Should().Be(newJourneyDate.Date);
        schedule.DepartureTime.Should().Be(newDepartureTime);
        schedule.ArrivalTime.Should().Be(newArrivalTime);
        schedule.Fare.Should().Be(newFare);
        schedule.UpdatedAt.Should().NotBeNull();
    }
}
