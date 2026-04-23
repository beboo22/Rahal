using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.SerpApiService;
using ApplicationBusiness.Dtos.Flights;
using ApplicationBusiness.Fetures.FlightService.Command.Model;
using ApplicationBusiness.Fetures.FlightService.Query.Model;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.FlightService
{
    internal class SearchFlightOrchestratorHandler
      : IQueryHandler<SearchFlightOrchestratorQuery, ApiResponse>
    {
        public ISender _mediator;
        private readonly ILogger<SearchFlightOrchestratorHandler> _logger;

        public SearchFlightOrchestratorHandler(
            ILogger<SearchFlightOrchestratorHandler> logger, 
            ISender mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<ApiResponse> Handle(
            SearchFlightOrchestratorQuery request,
            CancellationToken cancellationToken)
        {
            // 1. get flight data
            var flightResult = await _mediator.Send(
                new GetFlightQuery(request.Filter),
                cancellationToken);

            if (flightResult is not ApiResultResponse<FlightSearchResponse> response ||
                response.Data is null)
            {
                return flightResult;
            }

            if(response.statusCode == 224)
                return response;

            // 2. save in db
            var groupKey = CacheKeys.FlightsGroup(request.Filter);
            var exactKey = CacheKeys.FlightsExact(request.Filter);

            var saveResult = await _mediator.Send(
                new SaveFlightCommand(response.Data, exactKey, groupKey),
                cancellationToken);
            return saveResult;
            //if (flightResult is not ApiResultResponse<FlightSearchHistory> FlightSearchHistoryresponse ||
            //    response.Data is null)
            //{
            //    return flightResult;
            //}

            //if (saveResult.statusCode != 200)
            //{
            //    return saveResult;
            //}

            //// 3. merge result
            //return new ApiResultResponse<FlightSearchHistory>(200, FlightSearchHistoryresponse.Data);
        }
    }
}
