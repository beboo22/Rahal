using Application.Abstraction.message;
using ApplicationBusiness.Fetures.BookingTripService.Command.Models;
using ApplicationBusiness.Fetures.BookingTripService.Query.Models;
using ApplicationBusiness.Fetures.BookingTripService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.TripEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ApplicationBusiness.Fetures.Authentication.Command.Models;

namespace ApplicationBusiness.Fetures.BookingTripService.Query
{
    public record IsBookingExistToTrip(int TripId) : IQuery<ApiResponse>;
    internal class BookingTripQueryHandler : IQueryHandler<GetBookingById, ApiResponse>,
        IQueryHandler<GetAllBooking, ApiResponse>,
        IQueryHandler<ReturnMonyToUser, ApiResponse>,
        IQueryHandler<IsBookingExistToTrip, ApiResponse>

    {
        IReadGenericRepo<BookingPublicTrip> _RBTR { get; set; }
        public ISender Sender { get; set; }

        public BookingTripQueryHandler(IReadGenericRepo<BookingPublicTrip> rBTR)
        {
            _RBTR = rBTR;
        }

        public async Task<ApiResponse> Handle(GetAllBooking request, CancellationToken cancellationToken)
        {
            var booking = await _RBTR.GetAll().Include(b => b.PublicTrip).AsNoTracking().Select(b => new BookingTripTemplate
            {
                PublicTripId =  b.PublicTripId,
                Id = b.Id,
                BookingDate = b.BookingDate,
                IsPaid = b.IsPaid,
                TotalBookingPrice = b.TotalBookingPrice,
                TripTilte = b.PublicTrip.Title,
            }).ToListAsync();
            
            if (booking != null)
                return new ApiResultResponse<List<BookingTripTemplate>>((int)HttpStatusCode.OK, booking);

            return new ApiResponse((int)HttpStatusCode.NotFound);
        }

        public async Task<ApiResponse> Handle(GetBookingById request, CancellationToken cancellationToken)
        {
            var booking = await _RBTR.GetByIdAsync(request.Id);
            var template = new BookingTripTemplate
            {
                Id=request.Id,
                BookingDate = booking.BookingDate,
                IsPaid = booking.IsPaid,
                TotalBookingPrice = booking.TotalBookingPrice,
                TripTilte = booking.PublicTrip.Title,
            };
            if (template != null)
                return new ApiResultResponse<BookingTripTemplate>((int)HttpStatusCode.Accepted, template);
            return new ApiResponse((int)HttpStatusCode.NotFound);

        }

        public async Task<ApiResponse> Handle(ReturnMonyToUser request, CancellationToken cancellationToken)
        {
            try
            {

                var bookings = _RBTR.GetAll()
                        .Include(b => b.User)
                        .Where(b => b.PublicTripId == request.TripId && b.IsPaid)
                        .ToList();

                foreach (var item in bookings)
                {
                    if (item.User.FinancialBalance.HasValue)
                        item.User.FinancialBalance += item.TotalBookingPrice;
                    else item.User.FinancialBalance = item.TotalBookingPrice;
                }

                var users = bookings.Select(b => b.User).ToList();
                //await _WUR.UpdateRangeAsync(users);
                return await Sender.Send(new UpdateUsers(users));
                
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, ex.Message);
            }

        }

        public async Task<ApiResponse> Handle(IsBookingExistToTrip request, CancellationToken cancellationToken)
        {
            var isExist = await _RBTR.GetAll().AnyAsync(b => b.PublicTripId == request.TripId, cancellationToken);
            return new ApiResultResponse<bool>((int)HttpStatusCode.OK, isExist);
        }
    }
}
