using Application.Abstraction.message;
using ApplicationBusiness.Fetures.BookingTripService.Query.Models;
using ApplicationBusiness.Fetures.BookingTripService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.TripEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.BookingTripService.Query
{
    internal class BookingTripQueryHandler : IQueryHandler<GetBookingById, ApiResponse>, IQueryHandler<GetAllBooking, ApiResponse>
    {
        IReadGenericRepo<BookingTrip> _RBTR { get; set; }

        public BookingTripQueryHandler(IReadGenericRepo<BookingTrip> rBTR)
        {
            _RBTR = rBTR;
        }

        public async Task<ApiResponse> Handle(GetAllBooking request, CancellationToken cancellationToken)
        {
            var booking = await _RBTR.GetAll().Include(b => b.PublicTrip).AsNoTracking().Select(b => new BookingTripTemplate
            {
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
    }
}
