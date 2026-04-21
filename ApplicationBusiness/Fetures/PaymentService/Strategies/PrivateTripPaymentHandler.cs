using Domain.Abstraction;
using Domain.Entity.TripEntity;
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

        public PaymentEntityType Type => PaymentEntityType.PrivateTrip;

        public PrivateTripPaymentHandler(IWriteGenericRepo<BookingPrivateTrip> repo, IReadGenericRepo<BookingPrivateTrip> rrepo)
        {
            _wrepo = repo;
            _rrepo = rrepo;
        }

        public async Task HandleAsync(int entityId, bool success)
        {
            var booking = await _rrepo.GetByIdAsync(entityId);

            if (booking == null)
                throw new Exception("Private booking not found");

            booking.IsPaid = success;

            await _wrepo.UpdateAsync(booking, entityId);
        }
    }
}
