using Domain.Entities;
using Domain.Services;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Tests;

public class SeatBookingDomainServiceTests
{
    private readonly SeatBookingDomainService _domainService;

    public SeatBookingDomainServiceTests()
    {
        _domainService = new SeatBookingDomainService();
    }

    [Fact]
    public void CanBookSeat_WithAvailableSeat_ShouldReturnTrue()
    {
        // Arrange
        var seat = new Seat(Guid.NewGuid(), "1", "1");

        // Act
        var canBook = _domainService.CanBookSeat(seat);

        // Assert
        canBook.Should().BeTrue();
    }

    [Fact]
    public void CanBookSeat_WithBookedSeat_ShouldReturnFalse()
    {
        // Arrange
        var seat = new Seat(Guid.NewGuid(), "1", "1");
        seat.Book();

        // Act
        var canBook = _domainService.CanBookSeat(seat);

        // Assert
        canBook.Should().BeFalse();
    }

    [Fact]
    public void CanBookSeat_WithSoldSeat_ShouldReturnFalse()
    {
        // Arrange
        var seat = new Seat(Guid.NewGuid(), "1", "1");
        seat.Book();
        seat.MarkAsSold();

        // Act
        var canBook = _domainService.CanBookSeat(seat);

        // Assert
        canBook.Should().BeFalse();
    }

    [Fact]
    public async Task BookSeatAsync_WithAvailableSeat_ShouldCreateTicketAndBookSeat()
    {
        // Arrange
        var seat = new Seat(Guid.NewGuid(), "1", "1");
        var passenger = new Passenger("John Doe", "1234567890", "john@test.com");
        var boardingPoint = "New York";
        var droppingPoint = "Boston";
        var fare = 45m;

        // Use reflection to set passenger ID
        var passengerIdProperty = typeof(Passenger).GetProperty("Id");
        passengerIdProperty?.SetValue(passenger, Guid.NewGuid());

        // Use reflection to set seat ID
        var seatIdProperty = typeof(Seat).GetProperty("Id");
        var seatId = Guid.NewGuid();
        seatIdProperty?.SetValue(seat, seatId);

        // Act
        var ticket = await _domainService.BookSeatAsync(seat, passenger, boardingPoint, droppingPoint, fare);

        // Assert
        ticket.Should().NotBeNull();
        ticket.SeatId.Should().Be(seatId);
        ticket.BoardingPoint.Should().Be(boardingPoint);
        ticket.DroppingPoint.Should().Be(droppingPoint);
        ticket.TotalAmount.Should().Be(fare);
        seat.Status.Should().Be(SeatStatus.Booked);
    }

    [Fact]
    public async Task BookSeatAsync_WithBookedSeat_ShouldThrowException()
    {
        // Arrange
        var seat = new Seat(Guid.NewGuid(), "1", "1");
        seat.Book(); // Already booked

        var passenger = new Passenger("John Doe", "1234567890", "john@test.com");

        // Act
        var act = async () => await _domainService.BookSeatAsync(
            seat, passenger, "NY", "Boston", 45m);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*cannot be booked*");
    }

    [Fact]
    public async Task BookSeatAsync_WithSoldSeat_ShouldThrowException()
    {
        // Arrange
        var seat = new Seat(Guid.NewGuid(), "1", "1");
        seat.Book();
        seat.MarkAsSold(); // Already sold

        var passenger = new Passenger("John Doe", "1234567890", "john@test.com");

        // Act
        var act = async () => await _domainService.BookSeatAsync(
            seat, passenger, "NY", "Boston", 45m);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*cannot be booked*");
    }
}
