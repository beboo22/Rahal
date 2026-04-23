using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.SerpApiService;
using ApplicationBusiness.Fetures.HotelService.Query.Model;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.HotelService.Query
{

    internal class GetHotelsQueryHandler
    : IQueryHandler<GetHotelsQuery, ApiResponse>
    {
        private readonly ISerpApiService _serpApiService;

        public GetHotelsQueryHandler(ISerpApiService serpApiService)
        {
            _serpApiService = serpApiService;
        }

        public async Task<ApiResponse> Handle(
            GetHotelsQuery request,
            CancellationToken cancellationToken)
        {
             return await _serpApiService.SearchHotelsAsync(
                request.Request,
                cancellationToken);
        }
    }

}
