using Application.Abstraction.message;
using ApplicationBusiness.Fetures.BookingTripService.Command.Models;
using ApplicationBusiness.Fetures.BookingTripService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.BookingTripService.Command
{
    internal class BookingTripCommandHandler : ICommandHandler<BookTrip, ApiResponse>,ICommandHandler<DeleteBookTrip, ApiResponse>
    {
        public IWriteGenericRepo<BookingTrip> _WBTR { get; set; }
        public IWriteGenericRepo<User> _WUR { get; set; }
        public IReadGenericRepo<User> _RUR { get; set; }
        public IWriteGenericRepo<PublicTrip> _WTR { get; set; }
        public IReadGenericRepo<BookingTrip> _RBTR { get; set; }
        public IReadGenericRepo<PublicTrip> _RTR { get; set; }

        public IWriteUnitOfWork _uof { get; set; }

        public BookingTripCommandHandler(IWriteGenericRepo<BookingTrip> wBTR, IReadGenericRepo<BookingTrip> rBTR, IWriteUnitOfWork uof, IWriteGenericRepo<User> wUR, IWriteGenericRepo<PublicTrip> wTR, IReadGenericRepo<PublicTrip> rTR, IReadGenericRepo<User> rUR)
        {
            _WBTR = wBTR;
            _RBTR = rBTR;
            _uof = uof;
            _WUR = wUR;
            _WTR = wTR;
            _RTR = rTR;
            _RUR = rUR;
        }

        public async Task<ApiResponse> Handle(BookTrip request, CancellationToken cancellationToken)
        {
            await _uof.BeginTransiaction();
            try
            {
                var Check = await _RTR.GetByIdAsync(request.TripId);
                if (Check is null)
                {
                    return new ApiResponse((int)HttpStatusCode.NotFound, "Trip not found");
                }
                var CheckUser = await _WUR.ExistsAsync(request.UserId);
                if (!CheckUser)
                {
                    return new ApiResponse((int)HttpStatusCode.NotFound, "User not found");
                }
                if (Check.CreatedById == request.UserId)
                    return new ApiResponse((int)HttpStatusCode.Conflict, "User who create trip can't book it");
                var Trip = await _RTR.GetByIdAsync(request.TripId);
                var entity = new BookingTrip()
                {
                    PublicTripId = request.TripId,
                    UserId = request.UserId,
                };
                entity.TotalBookingPrice = Trip.Price + Trip.TravelerFee;
                await _WBTR.AddAsync(entity);
                await _uof.SaveChangesAsync();

                var item = new BookingTripTemplate()
                {
                    TripTilte = Trip.Title,
                    BookingDate = entity.BookingDate,
                    TotalBookingPrice = entity.TotalBookingPrice,
                    IsPaid = entity.IsPaid
                };
                return new ApiResultResponse<BookingTripTemplate>((int)HttpStatusCode.Created,item,"Booking trip created successfully");
            }
            catch (Exception ex)
            {
                await _uof.RollbackAsync();
                return new ApiResponse(500,ex.Message);

            }
        }

        public async Task<ApiResponse> Handle(DeleteBookTrip request, CancellationToken cancellationToken)
        {
            await _uof.BeginTransiaction();
            try
            {
                //get userid from book
                var book = await _RBTR.GetByIdAsync(request.BookingId);
                if (book == null)
                    return new ApiResponse((int)HttpStatusCode.NotFound);
                var user = await _RUR.GetByIdAsync(book.UserId);
                if(book.CreatedAt.AddDays(1) <  DateTime.UtcNow)
                    user.FinancialBalance += book.TotalBookingPrice - (book.TotalBookingPrice * 0.05m);// get 5% fee for cancle booking
                else
                    user.FinancialBalance = book.TotalBookingPrice;
                /**
                 * we should send the money of fee to our app cridit bank in future             
                 */
                await Task.WhenAll(_WUR.UpdateAsync(user, user.Id), _WBTR.DeleteAsync(request.BookingId));
                await _uof.SaveChangesAsync();
                return new ApiResponse((int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                await _uof.RollbackAsync();
                return new ApiResponse(500,ex.Message);
            }
        }
    }
}
