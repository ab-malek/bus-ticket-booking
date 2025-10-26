using Application.Contracts.DTOs;
using Application.Contracts.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests;

public class BookingServiceTests
{
    private readonly Mock<IBusScheduleRepository> _busScheduleRepositoryMock;
    private readonly Mock<ISeatRepository> _seatRepositoryMock;
    private readonly Mock<IPassengerRepository> _passengerRepositoryMock;
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;
    private readonly ISeatBookingDomainService _seatBookingDomainService;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _busScheduleRepositoryMock = new Mock<IBusScheduleRepository>();
        _seatRepositoryMock = new Mock<ISeatRepository>();
        _passengerRepositoryMock = new Mock<IPassengerRepository>();
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _seatBookingDomainService = new SeatBookingDomainService();

        _bookingService = new BookingService(
            _busScheduleRepositoryMock.Object,
            _seatRepositoryMock.Object,
            _passengerRepositoryMock.Object,
            _ticketRepositoryMock.Object,
            _seatBookingDomainService
        );
    }

    [Fact]
    public async Task BookSeatAsync_WithAvailableSeat_ShouldReturnSuccess()
    {
        // Arrange
        var busScheduleId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var busId = Guid.NewGuid();
        var routeId = Guid.NewGuid();

        var bus = new Bus("Test Company", "Test Bus", "TB-123", "AC", 40);
        var seat = new Seat(busScheduleId, "1", "1");
        var busSchedule = new BusSchedule(
            busId, routeId,
            DateTime.Today, TimeSpan.FromHours(8),
            TimeSpan.FromHours(12), 45m,
            "Boarding", "Dropping"
        );

        var input = new BookSeatInputDto
        {
            BusScheduleId = busScheduleId,
            SeatId = seatId,
            PassengerName = "John Doe",
            MobileNumber = "1234567890",
            BoardingPoint = "Boarding",
            DroppingPoint = "Dropping"
        };

        _seatRepositoryMock
            .Setup(r => r.GetWithDetailsAsync(seatId))
            .ReturnsAsync(seat);

        _passengerRepositoryMock
            .Setup(r => r.FindByMobileNumberAsync(input.MobileNumber))
            .ReturnsAsync((Passenger?)null);

        _passengerRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Passenger>()))
            .ReturnsAsync((Passenger p) => p);

        _busScheduleRepositoryMock
            .Setup(r => r.GetByIdAsync(busScheduleId))
            .ReturnsAsync(busSchedule);

        _ticketRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Ticket>()))
            .ReturnsAsync((Ticket t) => t);

        _seatRepositoryMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _bookingService.BookSeatAsync(input);

        // Assert
        result.Success.Should().BeTrue();
        result.BookingReference.Should().NotBeNullOrEmpty();
        result.TotalAmount.Should().Be(45m);
        seat.Status.Should().Be(SeatStatus.Sold);
    }

    [Fact]
    public async Task BookSeatAsync_WithBookedSeat_ShouldReturnFailure()
    {
        // Arrange
        var busScheduleId = Guid.NewGuid();
        var seatId = Guid.NewGuid();

        var seat = new Seat(busScheduleId, "1", "1");
        seat.Book(); // Mark seat as booked

        var input = new BookSeatInputDto
        {
            BusScheduleId = busScheduleId,
            SeatId = seatId,
            PassengerName = "John Doe",
            MobileNumber = "1234567890",
            BoardingPoint = "Boarding",
            DroppingPoint = "Dropping"
        };

        _seatRepositoryMock
            .Setup(r => r.GetWithDetailsAsync(seatId))
            .ReturnsAsync(seat);

        // Act
        var result = await _bookingService.BookSeatAsync(input);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not available");
    }

    [Fact]
    public async Task GetSeatPlanAsync_WithValidBusSchedule_ShouldReturnSeatPlan()
    {
        // Arrange
        var busScheduleId = Guid.NewGuid();
        var busId = Guid.NewGuid();
        var routeId = Guid.NewGuid();

        var bus = new Bus("Test Company", "Test Bus", "TB-123", "AC", 40);
        var busSchedule = new BusSchedule(
            busId, routeId,
            DateTime.Today, TimeSpan.FromHours(8),
            TimeSpan.FromHours(12), 45m,
            "Boarding", "Dropping"
        );
        busSchedule.InitializeSeats(40);

        // Use reflection to set the Id and Bus navigation property
        var idProperty = typeof(BusSchedule).GetProperty("Id");
        idProperty?.SetValue(busSchedule, busScheduleId);
        var busProperty = typeof(BusSchedule).GetProperty("Bus");
        busProperty?.SetValue(busSchedule, bus); _busScheduleRepositoryMock
            .Setup(r => r.GetWithSeatsAsync(busScheduleId))
            .ReturnsAsync(busSchedule);

        // Act
        var result = await _bookingService.GetSeatPlanAsync(busScheduleId);

        // Assert
        result.Should().NotBeNull();
        result.BusScheduleId.Should().Be(busScheduleId);
        result.Seats.Should().HaveCount(40);
        result.Seats.Should().AllSatisfy(s => s.Status.Should().Be(SeatStatus.Available));
    }
}
