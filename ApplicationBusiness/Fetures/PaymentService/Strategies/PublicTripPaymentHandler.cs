using ApplicationBusiness.Fetures.NotficationSystem.Command.Models;
using Domain.Abstraction;
using Domain.Entity.TripEntity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PaymentService.Strategies
{
    public class PublicTripPaymentHandler : IPaymentHandlerStrategy
    {
        private readonly IWriteGenericRepo<BookingPublicTrip> _wrepo;
        private readonly IReadGenericRepo<BookingPublicTrip> _rrepo;
        private readonly IWriteUnitOfWork _writeUnitOfWork;
        public ISender Sender { get; set; }


        public PaymentEntityType Type => PaymentEntityType.PublicTrip;

        public PublicTripPaymentHandler(IWriteGenericRepo<BookingPublicTrip> repo, IReadGenericRepo<BookingPublicTrip> rrepo, IWriteUnitOfWork writeUnitOfWork)
        {
            _wrepo = repo;
            _rrepo = rrepo;
            _writeUnitOfWork = writeUnitOfWork;
        }

        public async Task HandleAsync(int entityId, bool success)
        {
            var booking = await _rrepo.GetByIdAsync(entityId);

            if (booking == null)
                throw new Exception("Public booking not found");

            booking.IsPaid = success;
            try
            {
                Console.WriteLine(_rrepo.GetHashCode());
                Console.WriteLine(_wrepo.GetHashCode());

                await _writeUnitOfWork.BeginTransactionAsync();
                await _wrepo.UpdateAsync(booking, entityId);
                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();
                await Sender.Send(
            new SendPaymentNotificationCommand(
                booking.UserId.ToString(),
                "success New payment for booking Public trip🔥",
                $"{booking.BookingDate}.",
                ""
            ));
            }
            catch (Exception ex)
            {
                await _writeUnitOfWork.RollbackAsync();
            }

        }
    }
}
