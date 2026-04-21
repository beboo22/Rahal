using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.Amadeus
{
    public class HotelSearchCache:BaseEntity
    {
        public int Id { get; set; }

        public string RouteKey { get; set; }

        public string ResponseJson { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
