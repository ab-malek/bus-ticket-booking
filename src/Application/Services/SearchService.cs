using Application.Contracts.DTOs;
using Application.Contracts.Repositories;
using Domain.Enums;

namespace Application.Services;

public interface ISearchService
{
    Task<List<AvailableBusDto>> SearchAvailableBusesAsync(string from, string to, DateTime journeyDate);
    Task<AvailableBusDto?> GetBusByScheduleIdAsync(Guid busScheduleId);
}

public class SearchService : ISearchService
{
    private readonly IBusScheduleRepository _busScheduleRepository;

    public SearchService(IBusScheduleRepository busScheduleRepository)
    {
        _busScheduleRepository = busScheduleRepository;
    }

    public async Task<List<AvailableBusDto>> SearchAvailableBusesAsync(string from, string to, DateTime journeyDate)
    {
        if (string.IsNullOrWhiteSpace(from))
            throw new ArgumentException("From city cannot be empty", nameof(from));

        if (string.IsNullOrWhiteSpace(to))
            throw new ArgumentException("To city cannot be empty", nameof(to));

        var busSchedules = await _busScheduleRepository.SearchAvailableBusesAsync(from, to, journeyDate.Date);

        var result = busSchedules.Select(schedule =>
        {
            var totalSeats = schedule.Bus.TotalSeats;
            var bookedSeats = schedule.Seats.Count(s => s.Status == SeatStatus.Booked || s.Status == SeatStatus.Sold);
            var seatsLeft = totalSeats - bookedSeats;

            return new AvailableBusDto
            {
                BusScheduleId = schedule.Id,
                CompanyName = schedule.Bus.CompanyName,
                BusName = schedule.Bus.BusName,
                BusType = schedule.Bus.BusType,
                JourneyDate = schedule.JourneyDate,
                DepartureTime = schedule.DepartureTime,
                ArrivalTime = schedule.ArrivalTime,
                TotalSeats = totalSeats,
                BookedSeats = bookedSeats,
                SeatsLeft = seatsLeft,
                Price = schedule.Fare,
                BoardingPoint = schedule.BoardingPoint,
                DroppingPoint = schedule.DroppingPoint
            };
        }).ToList();

        return result;
    }

    public async Task<AvailableBusDto?> GetBusByScheduleIdAsync(Guid busScheduleId)
    {
        var schedule = await _busScheduleRepository.GetWithDetailsAsync(busScheduleId);

        if (schedule == null)
            return null;

        var totalSeats = schedule.Bus.TotalSeats;
        var bookedSeats = schedule.Seats.Count(s => s.Status == SeatStatus.Booked || s.Status == SeatStatus.Sold);
        var seatsLeft = totalSeats - bookedSeats;

        return new AvailableBusDto
        {
            BusScheduleId = schedule.Id,
            CompanyName = schedule.Bus.CompanyName,
            BusName = schedule.Bus.BusName,
            BusType = schedule.Bus.BusType,
            JourneyDate = schedule.JourneyDate,
            DepartureTime = schedule.DepartureTime,
            ArrivalTime = schedule.ArrivalTime,
            TotalSeats = totalSeats,
            BookedSeats = bookedSeats,
            SeatsLeft = seatsLeft,
            Price = schedule.Fare,
            BoardingPoint = schedule.BoardingPoint,
            DroppingPoint = schedule.DroppingPoint
        };
    }
}
