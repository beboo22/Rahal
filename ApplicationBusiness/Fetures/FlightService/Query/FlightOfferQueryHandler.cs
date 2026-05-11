using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.spacification;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.FlightService.Query
{
    public record GetFlightOffer(FlightHistoryFilter Filter) : IQuery<ApiResponse>;
    internal class FlightOfferQueryHandler : IQueryHandler<GetFlightOffer, ApiResponse>
    {
        private IReadGenericRepo<FlightOffer> Repo { get; set; }

        public FlightOfferQueryHandler(IReadGenericRepo<FlightOffer> repo)
        {
            Repo = repo;
        }

        public async Task<ApiResponse> Handle(GetFlightOffer request, CancellationToken cancellationToken)
        {


            var items = await Repo.GetAllSpec(new FlightSearchHistorySpecification(request.Filter)).ToListAsync();


            if (!items.Any())
                return new ApiResponse(404);
            if (request.Filter.Id.HasValue)
                return new ApiResultResponse<FlightOffer>(200, items.First());
            return new ApiResultResponse<List<FlightOffer>>(200, items);
        }
    }
}
