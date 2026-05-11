using Domain.Abstraction;
using Domain.Entity.Hotel_flights;
using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PaymentService.Strategies
{
    internal class FlightPaymentHandler : IPaymentHandlerStrategy
    {
        private readonly IWriteGenericRepo<PayFlight> _wrepo;
        private readonly IReadGenericRepo<PayFlight> _rrepo;
        private IWriteUnitOfWork _unitOfWork;
        public PaymentEntityType Type => PaymentEntityType.Flight;

        public FlightPaymentHandler(IWriteGenericRepo<PayFlight> repo, IReadGenericRepo<PayFlight> rrepo, IWriteUnitOfWork unitOfWork)
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
            try
            {

                booking.IsPaid = success;
                await _unitOfWork.BeginTransactionAsync();
                await _wrepo.UpdateAsync(booking, entityId);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
            await _unitOfWork.RollbackAsync();

            }




        }
    }
}
