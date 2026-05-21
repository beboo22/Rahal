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
        private IWriteUnitOfWork _unitOfWork;

        public PaymentEntityType Type => PaymentEntityType.PrivateTrip;

        public PrivateTripPaymentHandler(IWriteGenericRepo<BookingPrivateTrip> repo, IReadGenericRepo<BookingPrivateTrip> rrepo, IWriteUnitOfWork unitOfWork)
        {
            _wrepo = repo;
            _rrepo = rrepo;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(int entityId, bool success)
        {
            var booking = await _rrepo.GetByIdAsync(entityId);

            if (booking == null)
                throw new Exception("Private booking not found");

            booking.IsPaid = success;
            await _unitOfWork.BeginTransactionAsync();
            await _wrepo.UpdateAsync(booking, entityId);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
        }
    }
}
