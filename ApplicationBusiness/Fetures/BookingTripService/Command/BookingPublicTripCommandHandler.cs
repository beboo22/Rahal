using Application.Abstraction.message;
using Application.Fetures.Authentication.Command.Models;
using Application.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Fetures.Authentication.Command.Models;
using ApplicationBusiness.Fetures.BookingTripService.Command.Models;
using ApplicationBusiness.Fetures.BookingTripService.Query.Response;
using ApplicationBusiness.Fetures.FlightService.Command;
using ApplicationBusiness.Fetures.HotelService.Command;
using ApplicationBusiness.Fetures.TripService.Query.Models;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TripEntity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.BookingTripService.Command
{
    internal class BookingPublicTripCommandHandler :
        ICommandHandler<BookTrip, ApiResponse>,
        ICommandHandler<DeleteBookTrip, ApiResponse>
    {
        public IWriteGenericRepo<BookingPublicTrip> _WBTR { get; set; }

        //public IWriteGenericRepo<User> _WUR { get; set; }
        //public IReadGenericRepo<User> _RUR { get; set; }


        public IReadGenericRepo<BookingPublicTrip> _RBTR { get; set; }
        public IWriteUnitOfWork _uof { get; set; }
        public ISender Sender { get; set; }

        public BookingPublicTripCommandHandler(IWriteGenericRepo<BookingPublicTrip> wBTR,
            //IWriteGenericRepo<User> wUR, IReadGenericRepo<User> rUR,
            IReadGenericRepo<BookingPublicTrip> rBTR, IWriteUnitOfWork uof, ISender sender)
        {
            _WBTR = wBTR;
            //_WUR = wUR;
            //_RUR = rUR;
            _RBTR = rBTR;
            _uof = uof;
            Sender = sender;
        }



        public async Task<ApiResponse> Handle(BookTrip request, CancellationToken cancellationToken)
        {
            await _uof.BeginTransactionAsync();
            try
            {

                var Trip = await Sender.Send(new GetPubTripSpecQuery(new Abstraction.spacification.TripFilter
                {
                    Id = request.TripId,
                })) as ApiResultResponse<TemplateTrip>;



                if (Trip?.Data is null)
                {
                    return new ApiResponse((int)HttpStatusCode.NotFound, "Trip not found");
                }
                var CheckUser = await Sender.Send(new IsUserExist(request.UserId));
                //await _WUR.ExistsAsync(request.UserId);
                if (CheckUser.statusCode != 200)
                {
                    return new ApiResponse((int)HttpStatusCode.NotFound, "User not found");
                }
                if (Trip.Data.CreatedById == request.UserId)
                    return new ApiResponse((int)HttpStatusCode.Conflict, "User who create trip can't book it");
                //var Trip = await _RTR.GetByIdAsync(request.TripId);

                if (request.HotelId.HasValue)
                {

                    var CheckHotel = await Sender.Send(new CheckHotelExsist(request.HotelId.Value));
                    if (CheckHotel.statusCode != StatusCodes.Status302Found)
                    {
                        return new ApiResponse((int)HttpStatusCode.NotFound, "hotel not found");
                    }
                }
                if (request.flightId.HasValue)
                {

                    var CheckHotel = await Sender.Send(new CheckFlightExsist(request.HotelId.Value));
                    if (CheckHotel.statusCode != StatusCodes.Status302Found)
                    {
                        return new ApiResponse((int)HttpStatusCode.NotFound, "Flight not found");
                    }
                }



                var entity = new BookingPublicTrip()
                {
                    PublicTripId = request.TripId,
                    UserId = request.UserId,
                    HotelsId = request.HotelId,
                    FlightOffersId = request.flightId,
                };
                entity.TotalBookingPrice = Trip.Data.Price + Trip.Data.TravelerFee;
                await _WBTR.AddAsync(entity);
                await _uof.SaveChangesAsync();
                await _uof.CommitAsync();

                var item = new BookingTripTemplate()
                {
                    Id = entity.Id,

                    TripTilte = Trip.Data.Title,
                    BookingDate = entity.BookingDate,
                    TotalBookingPrice = entity.TotalBookingPrice,
                    IsPaid = false
                };
                return new ApiResultResponse<BookingTripTemplate>((int)HttpStatusCode.Created, item, "Booking trip created successfully");
            }
            catch (Exception ex)
            {
                await _uof.RollbackAsync();
                return new ApiResponse(500, ex.Message);

            }
        }

        public async Task<ApiResponse> Handle(DeleteBookTrip request, CancellationToken cancellationToken)
        {
            try
            {
                //get userid from book
                var book = await _RBTR.GetByIdAsync(request.BookingId);
                if (book == null)
                    return new ApiResponse((int)HttpStatusCode.NotFound);
                //var user = await _RUR.GetByIdAsync(book.UserId);
                var checkUserExitance = await Sender.Send(new GetUserById(book.UserId));
                if (checkUserExitance.statusCode != 200)
                {
                    return checkUserExitance;
                }
                var user = checkUserExitance as ApiResultResponse<User>;

                if (user.Data == null)
                {
                    return new ApiResponse(500, "Invalid user response");
                }


                if (book.CreatedAt.AddDays(1) < DateTime.UtcNow)
                    user.Data.FinancialBalance += book.TotalBookingPrice - (book.TotalBookingPrice * 0.05m);// get 5% fee for cancle booking
                else
                    user.Data.FinancialBalance = book.TotalBookingPrice;
                /**
                 * we should send the money of fee to our app cridit bank in future             
                 */
                //await Task.WhenAll(_WUR.UpdateAsync(user, user.Id),



                var updateuser = await Sender.Send(new UpdateUsers(new List<User>() { user.Data }));
                if (updateuser.statusCode != 200)
                    return updateuser;


                await _uof.BeginTransactionAsync();

                await _WBTR.DeleteAsync(request.BookingId);

                await _uof.SaveChangesAsync();
                await _uof.CommitAsync();
                return new ApiResponse((int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                await _uof.RollbackAsync();
                return new ApiResponse(500, ex.Message);
            }
        }

    }
}
