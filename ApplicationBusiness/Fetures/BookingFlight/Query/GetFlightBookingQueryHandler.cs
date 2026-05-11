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

namespace ApplicationBusiness.Fetures.BookingFlight.Query
{
    public record GetFlightBookingQuery(PaymentFilter Filter) : IQuery<ApiResponse>;

    internal class GetFlightBookingQueryHandler : IQueryHandler<GetFlightBookingQuery, ApiResponse>
    {
        private readonly IReadGenericRepo<PayFlight> _repo;

        public GetFlightBookingQueryHandler(IReadGenericRepo<PayFlight> repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse> Handle(GetFlightBookingQuery request, CancellationToken cancellationToken)
        {
            var flights = await _repo.GetAllSpec(new PayFlightSpecification(request.Filter)).ToListAsync();

            if (!flights.Any())
                return new ApiResponse(404);

            if (request.Filter.Id.HasValue)
            {
                return new ApiResultResponse<PayFlight>(200, flights.First());
            }

            return new ApiResultResponse<List<PayFlight>>(200, flights);
        }
    }
}
