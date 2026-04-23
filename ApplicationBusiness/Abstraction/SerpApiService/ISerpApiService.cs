using Application.Abstraction.Specification;
using ApplicationBusiness.Dtos.Flights;
using ApplicationBusiness.Dtos.Hotels;
using ApplicationBusiness.Dtos.Photos;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Abstraction.SerpApiService
{
    public interface ISerpApiService
    {
        Task<ApiResponse> SearchFlightsAsync(
            FlightSearchRequest request,
            CancellationToken cancellationToken = default);

        Task<ApiResponse> SearchHotelsAsync(
            HotelSearchRequest request,
            CancellationToken cancellationToken = default);
    }
    public interface ISerpPhotoApiService
    {
        Task<ApiResponse> SearchPhotoAsync(
            SearchPhotoReq request,
            CancellationToken cancellationToken = default);

    }

    
}
