using ApplicationBusiness.Fetures.NotficationSystem.Command.Models;
using Domain.Abstraction;
using Domain.Entity.Hotel_flights;
using Domain.Entity.TripEntity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PaymentService.Strategies
{
    internal class HotelPaymentHandler : IPaymentHandlerStrategy
    {
        private readonly IWriteGenericRepo<PayHotel> _wrepo;
        private readonly IReadGenericRepo<PayHotel> _rrepo;
        private IWriteUnitOfWork _unitOfWork;
        public ISender Sender { get; set; }

        public PaymentEntityType Type => PaymentEntityType.Hotel;

        public HotelPaymentHandler(IWriteGenericRepo<PayHotel> repo, IReadGenericRepo<PayHotel> rrepo, IWriteUnitOfWork unitOfWork, ISender sender)
        {
            _wrepo = repo;
            _rrepo = rrepo;
            _unitOfWork = unitOfWork;
            Sender = sender;
        }

        public async Task HandleAsync(int entityId, bool success)
        {
            var booking = await _rrepo.GetByIdAsync(entityId);

            if (booking == null)
                throw new Exception("Private booking not found");

            try
            {

                booking.IsPaid = success;
                await _unitOfWork.BeginTransactionAsync();
                await _wrepo.UpdateAsync(booking, entityId);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                await Sender.Send(
            new SendPaymentNotificationCommand(
                booking.UserId.ToString(),
                "success New payment for hotel🔥",
                $"{booking.Hotel.Name}.🏨🏨🏨🏨🏨🏨🏨🏨🏨🏨🏨🏨🏨",
                ""
            ));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();

            }
        }
    }
}
