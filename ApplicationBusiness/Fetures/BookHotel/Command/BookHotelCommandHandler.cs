using Application.Abstraction.message;
using Application.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Fetures.Authentication.Query;
using ApplicationBusiness.Fetures.BookHotel.Command.Models;
using ApplicationBusiness.Fetures.HotelService.Query.Model;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Hotel_flights;
using Domain.Entity.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.BookHotel.Command
{
    internal class BookHotelCommandHandler : ICommandHandler<BookHotelcommand, ApiResponse>
    {
        private IWriteGenericRepo<PayHotel> writeGenericRepo;
        private IWriteUnitOfWork writeUnitOfWork;
        public ISender Sender { get; set; }

        public BookHotelCommandHandler(
            IWriteGenericRepo<PayHotel> writeGenericRepo,
            IWriteUnitOfWork writeUnitOfWork,
            ISender sender)
        {
            this.writeGenericRepo = writeGenericRepo;
            this.writeUnitOfWork = writeUnitOfWork;
            Sender = sender;
        }

        public async Task<ApiResponse> Handle(BookHotelcommand request, CancellationToken cancellationToken)
        {
            var hotelres =  await Sender.Send(new GetHotelsspecQuery(new Abstraction.spacification.HotelHistoryFilter { Id = request.HotleId }));

            if(hotelres.statusCode != 200) 
                return hotelres;


            var hotel = hotelres as ApiResultResponse<Hotel>;

            if (hotel?.Data == null)
            {
                return new ApiResponse(500, "Invalid Hotel response");
            }

            var checkUserExitance = await Sender.Send(new GetUserById(request.UserId));
            if (checkUserExitance.statusCode != 200)
            {
                return checkUserExitance;
            }
            var user = checkUserExitance as ApiResultResponse<TemplateGenericProfile>;

            if (user?.Data == null)
            {
                return new ApiResponse(500, "Invalid user response");
            }

            try
            {

                await writeUnitOfWork.BeginTransactionAsync();
                var item = new PayHotel
                {
                    UserId = request.UserId,
                    HotelId = request.HotleId,
                    IsPaid = false,
                    Canceled = false,
                    TotalBookingPrice = hotel.Data.LowestPrice,
                };
                await writeGenericRepo.AddAsync(item);
                await writeUnitOfWork.SaveChangesAsync();
                await writeUnitOfWork.CommitAsync();
                return new ApiResultResponse<PayHotel>(200,item);
            }
            catch (Exception ex)
            {
                await writeUnitOfWork.RollbackAsync();
                return new ApiResponse(500,$"error while book: {ex.Message}");
            }

        }
    }
}
