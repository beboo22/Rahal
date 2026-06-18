using ApplicationBusiness.Fetures.NotficationSystem.Command.Models;
using ApplicationBusiness.Fetures.Profile.Command;
using Domain.Abstraction;
using Domain.Entity.Identity;
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
    public class PrivateTripPaymentHandler : IPaymentHandlerStrategy
    {
        private readonly IWriteGenericRepo<BookingPrivateTrip> _wrepo;
        private readonly IReadGenericRepo<BookingPrivateTrip> _rrepo;
        private IWriteUnitOfWork _unitOfWork;
        public ISender Sender { get; set; }

        public PaymentEntityType Type => PaymentEntityType.PrivateTrip;

        public PrivateTripPaymentHandler(IWriteGenericRepo<BookingPrivateTrip> repo, IReadGenericRepo<BookingPrivateTrip> rrepo, IWriteUnitOfWork unitOfWork, ISender sender)
        {
            _wrepo = repo;
            _rrepo = rrepo;
            _unitOfWork = unitOfWork;
            Sender = sender;
        }

        public async Task HandleAsync(int entityId, bool success)
        {
            var booking = await _rrepo.GetAll().Include(x => x.PrivateTrip).Where(x => x.Id == entityId).FirstOrDefaultAsync();


            if (booking == null)
                throw new Exception("Private booking not found");

            booking.IsPaid = success;
            await _unitOfWork.BeginTransactionAsync();
            await _wrepo.UpdateAsync(booking, entityId);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            if (booking.PrivateTrip.TourGuideId.HasValue)
                await Sender.Send(new AddEarnToTourguide(booking.PrivateTrip.TourGuideId.Value, booking.TotalOwnerProfit));
            await Sender.Send(
                    new SendPaymentNotificationCommand(
                        booking.UserId.ToString(),
                        "success New payment for booking private trip🔥",
                        $"{booking.BookingDate}.",
                        ""
                    ));
        }
    }
}
