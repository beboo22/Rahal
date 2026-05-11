using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Fetures.BookHotel.Query.Models;
using ApplicationBusiness.Fetures.BookingTripService.Query.Models;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.BookHotel.Query.Response
{
    internal class BookHotelQueryHandler : IQueryHandler<GetHotelBooking, ApiResponse>
    {
        IReadGenericRepo<PayHotel> Repo;

        public BookHotelQueryHandler(IReadGenericRepo<PayHotel> repo)
        {
            Repo = repo;
        }

        public async Task<ApiResponse> Handle(GetHotelBooking request, CancellationToken cancellationToken)
        {

            var hotel = await Repo.GetAllSpec(new PayHotelSpecification(request.Filter)).ToListAsync();


            if (!hotel.Any())
                return new ApiResponse(404);
            if (request.Filter.Id.HasValue)
            {
                return new ApiResultResponse<PayHotel>(200, hotel.First());
            }

            return new ApiResultResponse<List<PayHotel>>(200, hotel);




        }
    }
}
