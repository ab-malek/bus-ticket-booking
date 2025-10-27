using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Tests;

public class SeatTests
{
    [Fact]
    public void Seat_Creation_ShouldSetStatusAsAvailable()
    {
        // Arrange & Act
        var busScheduleId = Guid.NewGuid();
        var seat = new Seat(busScheduleId, "1", "1");

        // Assert
        seat.SeatNumber.Should().Be("1");
        seat.Row.Should().Be("1");
        seat.Status.Should().Be(SeatStatus.Available);
        seat.BusScheduleId.Should().Be(busScheduleId);
    }

    [Fact]
    public void Seat_Creation_WithEmptyBusScheduleId_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new Seat(Guid.Empty, "1", "1");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Bus schedule ID cannot be empty*");
    }

    [Fact]
    public void Seat_Creation_WithEmptySeatNumber_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new Seat(Guid.NewGuid(), "", "1");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Seat number cannot be empty*");
    }

    [Fact]
    public void Book_AvailableSeat_ShouldChangeStatusToBooked()
    {
        // Arrange
        var seat = new Seat(Guid.NewGuid(), "1", "1");

        // Act
        seat.Book();

        // Assert
        seat.Status.Should().Be(SeatStatus.Booked);
    }

    [Fact]
    public void Book_AlreadyBookedSeat_ShouldThrowException()
    {
        // Arrange
        var seat = new Seat(Guid.NewGuid(), "1", "1");
        seat.Book();

        // Act
        var act = () => seat.Book();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot book seat 1*");
    }

    [Fact]
    public void MarkAsSold_BookedSeat_ShouldChangeStatusToSold()
    {
        // Arrange
        var seat = new Seat(Guid.NewGuid(), "1", "1");
        seat.Book();

        // Act
        seat.MarkAsSold();

        // Assert
        seat.Status.Should().Be(SeatStatus.Sold);
    }

    [Fact]
    public void MarkAsSold_AvailableSeat_ShouldThrowException()
    {
        // Arrange
        var seat = new Seat(Guid.NewGuid(), "1", "1");

        // Act
        var act = () => seat.MarkAsSold();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot mark seat 1 as sold*");
    }

    [Fact]
    public void SeatStateTransition_AvailableToBookedToSold_ShouldSucceed()
    {
        // Arrange
        var seat = new Seat(Guid.NewGuid(), "1", "1");

        // Act & Assert - Available
        seat.Status.Should().Be(SeatStatus.Available);

        // Act - Book
        seat.Book();
        seat.Status.Should().Be(SeatStatus.Booked);

        // Act - Mark as Sold
        seat.MarkAsSold();
        seat.Status.Should().Be(SeatStatus.Sold);
    }
}
