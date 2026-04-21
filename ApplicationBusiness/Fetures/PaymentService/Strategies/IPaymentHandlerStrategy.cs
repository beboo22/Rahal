using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PaymentService.Strategies
{
    public interface IPaymentHandlerStrategy
    {
        PaymentEntityType Type { get; }

        Task HandleAsync(int entityId, bool success);
    }
}
