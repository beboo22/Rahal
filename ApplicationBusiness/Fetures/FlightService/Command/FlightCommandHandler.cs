using Application.Abstraction.message;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.FlightService.Command
{
    public record CheckFlightExsist(int hotelId) : ICommand<ApiResponse>;

    internal class FlightCommandHandler : ICommandHandler<CheckFlightExsist, ApiResponse>
    {
        private IWriteGenericRepo<FlightOffer> writeGenericRepo;

        public async Task<ApiResponse> Handle(CheckFlightExsist request, CancellationToken cancellationToken)
        {
            if (await writeGenericRepo.ExistsAsync(request.hotelId))
                return new ApiResponse(StatusCodes.Status302Found);

            return new ApiResponse(StatusCodes.Status404NotFound);
        }
    }
}
