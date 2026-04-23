using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.SerpApiService;
using ApplicationBusiness.Dtos.Hotels;
using ApplicationBusiness.Fetures.HotelService.Command.Model;
using ApplicationBusiness.Fetures.HotelService.Query.Model;
using Domain.BaseResponce;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.HotelService
{

    internal class HotelSearchOrchestratorHandler
    : IQueryHandler<HotelSearchOrchestratorQuery, ApiResponse>
    {
        private readonly IMediator _mediator;

        public HotelSearchOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ApiResponse> Handle(
            HotelSearchOrchestratorQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Call external API
            var hotelResult = await _mediator.Send(
                new GetHotelsQuery(request.Request),
                cancellationToken);

            if (hotelResult is not ApiResultResponse<HotelSearchResponse> response ||
                response.Data is null)
            {
                return hotelResult;
            }

            if (response.statusCode == 224)
                return response;




            // 2. Save to DB
            var groupKey = CacheKeys.HotelsGroup(request.Request);
            var exactKey = CacheKeys.HotelsExact(request.Request);

            var saveResult = await _mediator.Send(
                new SaveHotelCommand(response.Data, exactKey, groupKey),
                cancellationToken);
            return saveResult;
            //if (saveResult.statusCode != 200)
            //    return saveResult;

            //// 3. Return combined response
            //return new ApiResultResponse<object>(200, new
            //{
            //    Hotels = response.Data,
            //    Saved = true
            //}, "Hotels retrieved successfully");
        }
    }

}
