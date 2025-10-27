using Application.Contracts.DTOs;
using Application.Contracts.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests;

public class SearchServiceTests
{
    private readonly Mock<IBusScheduleRepository> _busScheduleRepositoryMock;
    private readonly SearchService _searchService;

    public SearchServiceTests()
    {
        _busScheduleRepositoryMock = new Mock<IBusScheduleRepository>();
        _searchService = new SearchService(_busScheduleRepositoryMock.Object);
    }

    [Fact]
    public async Task SearchAvailableBusesAsync_WithValidInput_ShouldReturnBuses()
    {
        // Arrange
        var journeyDate = DateTime.Today.AddDays(1);
        var busId = Guid.NewGuid();
        var routeId = Guid.NewGuid();

        var bus = new Bus("Test Company", "Test Bus", "TB-123", "AC", 40);
        var busSchedule = new BusSchedule(
            busId, routeId,
            journeyDate,
            TimeSpan.FromHours(8),
            TimeSpan.FromHours(12),
            45m,
            "New York",
            "Boston"
        );
        busSchedule.InitializeSeats(40);

        // Set navigation properties using reflection
        var busProperty = typeof(BusSchedule).GetProperty("Bus");
        busProperty?.SetValue(busSchedule, bus);

        var schedules = new List<BusSchedule> { busSchedule };

        _busScheduleRepositoryMock
            .Setup(r => r.SearchAvailableBusesAsync("New York", "Boston", journeyDate))
            .ReturnsAsync(schedules);

        // Act
        var result = await _searchService.SearchAvailableBusesAsync("New York", "Boston", journeyDate);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].CompanyName.Should().Be("Test Company");
        result[0].BusName.Should().Be("Test Bus");
        result[0].TotalSeats.Should().Be(40);
        result[0].SeatsLeft.Should().Be(40);
        result[0].Price.Should().Be(45m);
    }

    [Fact]
    public async Task SearchAvailableBusesAsync_WithBookedSeats_ShouldCalculateCorrectSeatsLeft()
    {
        // Arrange
        var journeyDate = DateTime.Today.AddDays(1);
        var busId = Guid.NewGuid();
        var routeId = Guid.NewGuid();

        var bus = new Bus("Test Company", "Test Bus", "TB-123", "AC", 40);
        var busSchedule = new BusSchedule(
            busId, routeId,
            journeyDate,
            TimeSpan.FromHours(8),
            TimeSpan.FromHours(12),
            45m,
            "New York",
            "Boston"
        );
        busSchedule.InitializeSeats(40);

        // Book 5 seats
        var seats = busSchedule.Seats.Take(5).ToList();
        foreach (var seat in seats)
        {
            seat.Book();
            seat.MarkAsSold();
        }

        // Set navigation properties
        var busProperty = typeof(BusSchedule).GetProperty("Bus");
        busProperty?.SetValue(busSchedule, bus);

        var schedules = new List<BusSchedule> { busSchedule };

        _busScheduleRepositoryMock
            .Setup(r => r.SearchAvailableBusesAsync("New York", "Boston", journeyDate))
            .ReturnsAsync(schedules);

        // Act
        var result = await _searchService.SearchAvailableBusesAsync("New York", "Boston", journeyDate);

        // Assert
        result.Should().HaveCount(1);
        result[0].TotalSeats.Should().Be(40);
        result[0].BookedSeats.Should().Be(5);
        result[0].SeatsLeft.Should().Be(35);
    }

    [Fact]
    public async Task SearchAvailableBusesAsync_WithEmptyFrom_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = async () => await _searchService.SearchAvailableBusesAsync("", "Boston", DateTime.Today);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*From city cannot be empty*");
    }

    [Fact]
    public async Task SearchAvailableBusesAsync_WithEmptyTo_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = async () => await _searchService.SearchAvailableBusesAsync("New York", "", DateTime.Today);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*To city cannot be empty*");
    }

    [Fact]
    public async Task SearchAvailableBusesAsync_WithNoBuses_ShouldReturnEmptyList()
    {
        // Arrange
        var journeyDate = DateTime.Today.AddDays(1);

        _busScheduleRepositoryMock
            .Setup(r => r.SearchAvailableBusesAsync("New York", "Boston", journeyDate))
            .ReturnsAsync(new List<BusSchedule>());

        // Act
        var result = await _searchService.SearchAvailableBusesAsync("New York", "Boston", journeyDate);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBusByScheduleIdAsync_WithValidId_ShouldReturnBus()
    {
        // Arrange
        var busScheduleId = Guid.NewGuid();
        var busId = Guid.NewGuid();
        var routeId = Guid.NewGuid();

        var bus = new Bus("Test Company", "Test Bus", "TB-123", "AC", 40);
        var busSchedule = new BusSchedule(
            busId, routeId,
            DateTime.Today.AddDays(1),
            TimeSpan.FromHours(8),
            TimeSpan.FromHours(12),
            45m,
            "New York",
            "Boston"
        );
        busSchedule.InitializeSeats(40);

        // Set navigation properties
        var idProperty = typeof(BusSchedule).GetProperty("Id");
        idProperty?.SetValue(busSchedule, busScheduleId);
        var busProperty = typeof(BusSchedule).GetProperty("Bus");
        busProperty?.SetValue(busSchedule, bus);

        _busScheduleRepositoryMock
            .Setup(r => r.GetWithDetailsAsync(busScheduleId))
            .ReturnsAsync(busSchedule);

        // Act
        var result = await _searchService.GetBusByScheduleIdAsync(busScheduleId);

        // Assert
        result.Should().NotBeNull();
        result!.BusScheduleId.Should().Be(busScheduleId);
        result.CompanyName.Should().Be("Test Company");
        result.BusName.Should().Be("Test Bus");
    }

    [Fact]
    public async Task GetBusByScheduleIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var busScheduleId = Guid.NewGuid();

        _busScheduleRepositoryMock
            .Setup(r => r.GetWithDetailsAsync(busScheduleId))
            .ReturnsAsync((BusSchedule?)null);

        // Act
        var result = await _searchService.GetBusByScheduleIdAsync(busScheduleId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SearchAvailableBusesAsync_WithMultipleBuses_ShouldReturnAll()
    {
        // Arrange
        var journeyDate = DateTime.Today.AddDays(1);
        var busId1 = Guid.NewGuid();
        var busId2 = Guid.NewGuid();
        var routeId = Guid.NewGuid();

        var bus1 = new Bus("Company A", "Bus A", "A-123", "AC", 40);
        var bus2 = new Bus("Company B", "Bus B", "B-456", "Non-AC", 45);

        var schedule1 = new BusSchedule(busId1, routeId, journeyDate,
            TimeSpan.FromHours(8), TimeSpan.FromHours(12), 45m, "NY", "Boston");
        schedule1.InitializeSeats(40);

        var schedule2 = new BusSchedule(busId2, routeId, journeyDate,
            TimeSpan.FromHours(10), TimeSpan.FromHours(14), 40m, "NY", "Boston");
        schedule2.InitializeSeats(45);

        // Set navigation properties
        var busProperty1 = typeof(BusSchedule).GetProperty("Bus");
        busProperty1?.SetValue(schedule1, bus1);
        busProperty1?.SetValue(schedule2, bus2);

        var schedules = new List<BusSchedule> { schedule1, schedule2 };

        _busScheduleRepositoryMock
            .Setup(r => r.SearchAvailableBusesAsync("NY", "Boston", journeyDate))
            .ReturnsAsync(schedules);

        // Act
        var result = await _searchService.SearchAvailableBusesAsync("NY", "Boston", journeyDate);

        // Assert
        result.Should().HaveCount(2);
        result[0].CompanyName.Should().Be("Company A");
        result[1].CompanyName.Should().Be("Company B");
    }
}
