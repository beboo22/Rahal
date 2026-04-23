using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.SerpApiService;
using ApplicationBusiness.Dtos.Flights;
using ApplicationBusiness.Fetures.FlightService.Query.Model;
using Domain.BaseResponce;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.FlightService.Query
{
    internal class FlightQueryHandler : IQueryHandler<FlightQuery, ApiResponse>
    {
        private readonly ISerpApiService _serpApiService;
        private readonly ILogger<FlightQueryHandler> _logger;

        public FlightQueryHandler(ISerpApiService serpApiService, ILogger<FlightQueryHandler> logger)
        {
            _serpApiService = serpApiService;
            _logger = logger;
        }

        public async Task<ApiResponse> Handle(FlightQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Flight search: {Origin} → {Dest} on {Date}",
               request.filter.DepartureId, request.filter.ArrivalId, request.filter.OutboundDate);

            var result = await _serpApiService.SearchFlightsAsync(request.filter, cancellationToken);

            //if (result.statusCode != 200)
            //    return result.statusCode is not 200
            //        ? StatusCode(503, result)
            //        : BadRequest(result);

            // Persist search history (fire and forget — don't block response)
            //if (result is ApiResultResponse<FlightSearchResponse> response)

            return result;


        }
    }
    internal class GetFlightQueryHandler
    : IQueryHandler<GetFlightQuery, ApiResponse>
    {
        private readonly ISerpApiService _serpApiService;

        public GetFlightQueryHandler(ISerpApiService serpApiService)
        {
            _serpApiService = serpApiService;
        }

        public async Task<ApiResponse> Handle(
            GetFlightQuery request,
            CancellationToken cancellationToken)
        {
            return await _serpApiService.SearchFlightsAsync(
                request.Filter,
                cancellationToken);
        }
    }
}
