using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.SerpApiService;
using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Fetures.HotelService.Query.Model;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.HotelService.Query
{

    internal class GetHotelsBySpecQueryHandler
    : IQueryHandler<GetHotelsspecQuery, ApiResponse>
    {
        public IReadGenericRepo<Hotel> _readGenericRepo;

        public GetHotelsBySpecQueryHandler(IReadGenericRepo<Hotel> readGenericRepo)
        {
            _readGenericRepo = readGenericRepo;
        }

        public async Task<ApiResponse> Handle(GetHotelsspecQuery request, CancellationToken cancellationToken)
        {
            // -----------------------------
            // 1. Direct fetch by Id
            // -----------------------------
            if (request.Request.Id.HasValue)
            {
                var item = await _readGenericRepo
                    .GetByIdAsync(request.Request.Id.Value);

                if (item == null)
                    return new ApiResponse(404,"hotel not found");

                return new ApiResultResponse<Hotel>(200, item);
            }

            // -----------------------------
            // 2. Search flow
            // -----------------------------
            var items = await _readGenericRepo
                .GetAllSpec(new HotelSpecification(request.Request))
                .ToListAsync();

            if(!items.Any())
                    return new ApiResponse(404,"hotel not found");

            return new ApiResultResponse<List<Hotel>>(200, items);
        }
    }

}
