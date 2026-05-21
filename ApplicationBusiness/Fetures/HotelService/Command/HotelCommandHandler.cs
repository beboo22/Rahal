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

namespace ApplicationBusiness.Fetures.HotelService.Command
{

    public record CheckHotelExsist(int hotelId) : ICommand<ApiResponse>;

    internal class HotelCommandHandler : ICommandHandler<CheckHotelExsist, ApiResponse>
    {
        private IWriteGenericRepo<Hotel> writeGenericRepo;

        public HotelCommandHandler(IWriteGenericRepo<Hotel> writeGenericRepo)
        {
            this.writeGenericRepo = writeGenericRepo;
        }

        public async Task<ApiResponse> Handle(CheckHotelExsist request, CancellationToken cancellationToken)
        {
            if (await writeGenericRepo.ExistsAsync(request.hotelId))
                return new ApiResponse(StatusCodes.Status302Found);

            return new ApiResponse(StatusCodes.Status404NotFound);
        }
    }
}
