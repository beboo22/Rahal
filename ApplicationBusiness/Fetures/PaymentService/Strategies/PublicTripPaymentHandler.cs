using ApplicationBusiness.Fetures.NotficationSystem.Command.Models;
using ApplicationBusiness.Fetures.Profile.Command;
using Domain.Abstraction;
using Domain.Entity.TripEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

        public PublicTripPaymentHandler(IWriteGenericRepo<BookingPublicTrip> repo, IReadGenericRepo<BookingPublicTrip> rrepo, IWriteUnitOfWork writeUnitOfWork, ISender sender)
        {
            _wrepo = repo;
            _rrepo = rrepo;
            _writeUnitOfWork = writeUnitOfWork;
            Sender = sender;
        }

        public async Task HandleAsync(int entityId, bool success)
        {
            var booking =  await _rrepo.GetAll().Include(x=>x.PublicTrip).Where(x=>x.Id == entityId).FirstOrDefaultAsync();

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
                if(booking.PublicTrip.TourGuideId.HasValue)
                    await Sender.Send(new AddEarnToTourguide(booking.PublicTrip.TourGuideId.Value, booking.TotalOwnerProfit));
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
