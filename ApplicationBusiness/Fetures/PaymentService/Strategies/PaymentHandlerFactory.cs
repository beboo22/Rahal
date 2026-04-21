using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PaymentService.Strategies
{
    public class PaymentHandlerFactory
    {
        private readonly Dictionary<PaymentEntityType, IPaymentHandlerStrategy> _handlers;

        public PaymentHandlerFactory(IEnumerable<IPaymentHandlerStrategy> handlers)
        {
            _handlers = handlers.ToDictionary(h => h.Type);
        }

        public IPaymentHandlerStrategy GetHandler(PaymentEntityType type)
        {
            if (!_handlers.TryGetValue(type, out var handler))
                throw new Exception($"No handler for {type}");

            return handler;
        }
    }
}
