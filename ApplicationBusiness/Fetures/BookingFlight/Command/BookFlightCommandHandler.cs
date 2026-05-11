using Application.Abstraction.message;
using Application.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Fetures.BookingFlight.Query;
using ApplicationBusiness.Fetures.FlightService.Query;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.BookingFlight.Command
{
    public record BookFlightCommand(int UserId, int FlightOfferId) : ICommand<ApiResponse>;

    internal class BookFlightCommandHandler : ICommandHandler<BookFlightCommand, ApiResponse>
    {
        private readonly IWriteGenericRepo<PayFlight> _writeRepo;
        private readonly IWriteUnitOfWork _uow;
        private readonly ISender _sender;

        public BookFlightCommandHandler(
            IWriteGenericRepo<PayFlight> writeRepo,
            IWriteUnitOfWork uow,
            ISender sender)
        {
            _writeRepo = writeRepo;
            _uow = uow;
            _sender = sender;
        }

        public async Task<ApiResponse> Handle(BookFlightCommand request, CancellationToken cancellationToken)
        {
            // 1. Get Flight Offer details (assuming a similar spec query exists)
            var flightRes = await _sender.Send(new GetFlightOffer(new FlightHistoryFilter { Id = request.FlightOfferId }));

            if (flightRes.statusCode != 200)
                return flightRes;

            var flightOffer = flightRes as ApiResultResponse<FlightOffer>;
            if (flightOffer?.Data == null)
                return new ApiResponse(500, "Invalid Flight Offer response");

            // 2. Check User existence
            var userRes = await _sender.Send(new GetUserById(request.UserId));
            if (userRes.statusCode != 200)
                return userRes;

            try
            {
                await _uow.BeginTransactionAsync();

                var booking = new PayFlight
                {
                    FlightOfferId = request.FlightOfferId,
                    UserId = request.UserId,
                    IsPaid = false,
                    Canceled = false,
                    TotalBookingPrice = flightOffer.Data.Price // Adjust property name based on your FlightOffer model
                };

                await _writeRepo.AddAsync(booking);
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                return new ApiResultResponse<PayFlight>(200,booking);
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                return new ApiResponse(500, $"Error while booking flight: {ex.Message}");
            }
        }
    }
}
