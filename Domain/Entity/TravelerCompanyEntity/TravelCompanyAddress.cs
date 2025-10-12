using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.TravelerCompanyEntity
{
    public class TravelerCompanyAddress
    {
        public int Id { get; set; }
        public int TravelCompanyId { get; set; }
        public TravelCompany TravelCompany { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
    }
}
