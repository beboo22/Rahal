using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.TripEntity
{
    public class PaymentRequest:BaseEntity
    {
        public string ProviderRef { get; set; } = null!;

        public int EntityId { get; set; }

        public PaymentEntityType EntityType { get; set; }

        public bool IsPaid { get; set; }
    }
    public enum PaymentEntityType
    {
        PublicTrip = 1,
        PrivateTrip = 2,
        Hotel = 3,
        Flight = 4
    }
}
