using Application.Contracts.DTOs;
using Application.Contracts.Repositories;
using Domain.Entities;
using Domain.Services;
using System.Data;

namespace Application.Services;

public interface IBookingService
{
    Task<SeatPlanDto> GetSeatPlanAsync(Guid busScheduleId);
    Task<BookSeatResultDto> BookSeatAsync(BookSeatInputDto input);
}

public class BookingService : IBookingService
{
    private readonly IBusScheduleRepository _busScheduleRepository;
    private readonly ISeatRepository _seatRepository;
    private readonly IPassengerRepository _passengerRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly ISeatBookingDomainService _seatBookingDomainService;
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(
        IBusScheduleRepository busScheduleRepository,
        ISeatRepository seatRepository,
        IPassengerRepository passengerRepository,
        ITicketRepository ticketRepository,
        ISeatBookingDomainService seatBookingDomainService,
        IUnitOfWork unitOfWork)
    {
        _busScheduleRepository = busScheduleRepository;
        _seatRepository = seatRepository;
        _passengerRepository = passengerRepository;
        _ticketRepository = ticketRepository;
        _seatBookingDomainService = seatBookingDomainService;
        _unitOfWork = unitOfWork;
    }

    public async Task<SeatPlanDto> GetSeatPlanAsync(Guid busScheduleId)
    {
        var busSchedule = await _busScheduleRepository.GetWithSeatsAsync(busScheduleId);

        if (busSchedule == null)
            throw new InvalidOperationException($"Bus schedule with ID {busScheduleId} not found");

        var seatPlan = new SeatPlanDto
        {
            BusScheduleId = busSchedule.Id,
            BusName = busSchedule.Bus.BusName,
            CompanyName = busSchedule.Bus.CompanyName,
            TotalSeats = busSchedule.Bus.TotalSeats,
            JourneyDate = busSchedule.JourneyDate,
            DepartureTime = busSchedule.DepartureTime,
            ArrivalTime = busSchedule.ArrivalTime,
            Fare = busSchedule.Fare,
            BoardingPoint = busSchedule.BoardingPoint,
            DroppingPoint = busSchedule.DroppingPoint,
            Seats = busSchedule.Seats.Select(s => new SeatDto
            {
                SeatId = s.Id,
                SeatNumber = s.SeatNumber,
                Row = s.Row,
                Status = s.Status
            }).ToList()
        };

        return seatPlan;
    }

    public async Task<BookSeatResultDto> BookSeatAsync(BookSeatInputDto input)
    {
        try
        {
            // Validate input
            if (input.BusScheduleId == Guid.Empty)
                return new BookSeatResultDto { Success = false, Message = "Invalid bus schedule" };

            if (input.SeatId == Guid.Empty)
                return new BookSeatResultDto { Success = false, Message = "Invalid seat" };

            // Begin transaction with Serializable isolation level to prevent race conditions
            // This ensures no other transaction can read or modify the seat until this transaction completes
            await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                // Get seat with details - this will lock the row for the transaction
                var seat = await _seatRepository.GetWithDetailsAsync(input.SeatId);
                if (seat == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new BookSeatResultDto { Success = false, Message = "Seat not found" };
                }

                // Check if seat is available using domain service
                if (!_seatBookingDomainService.CanBookSeat(seat))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new BookSeatResultDto
                    {
                        Success = false,
                        Message = $"Seat {seat.SeatNumber} is not available. Current status: {seat.Status}"
                    };
                }

                // Get or create passenger
                var passenger = await _passengerRepository.FindByMobileNumberAsync(input.MobileNumber);
                if (passenger == null)
                {
                    passenger = new Passenger(input.PassengerName, input.MobileNumber, input.Email);
                    await _passengerRepository.AddAsync(passenger);
                }

                // Get bus schedule to get fare
                var busSchedule = await _busScheduleRepository.GetByIdAsync(input.BusScheduleId);
                if (busSchedule == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new BookSeatResultDto { Success = false, Message = "Bus schedule not found" };
                }

                // Book seat using domain service
                var ticket = await _seatBookingDomainService.BookSeatAsync(
                    seat,
                    passenger,
                    input.BoardingPoint,
                    input.DroppingPoint,
                    busSchedule.Fare);

                // Confirm the ticket
                ticket.Confirm();

                // Mark seat as sold
                seat.MarkAsSold();

                // Save ticket and seat
                await _ticketRepository.AddAsync(ticket);
                await _seatRepository.UpdateAsync(seat);

                // Commit transaction - this releases the locks
                await _unitOfWork.CommitTransactionAsync();

                return new BookSeatResultDto
                {
                    Success = true,
                    Message = "Seat booked successfully",
                    BookingReference = ticket.BookingReference,
                    TicketId = ticket.Id,
                    TotalAmount = ticket.TotalAmount
                };
            }
            catch
            {
                // Rollback on any error within the transaction
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        catch (InvalidOperationException ex)
        {
            return new BookSeatResultDto { Success = false, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new BookSeatResultDto
            {
                Success = false,
                Message = $"An error occurred while booking the seat: {ex.Message}"
            };
        }
    }
}
