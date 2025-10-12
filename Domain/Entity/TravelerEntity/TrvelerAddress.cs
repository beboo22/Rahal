using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.TravelerEntity
{
    public class TrvelerAddress
    {
        public int Id { get; set; }


        public int TravelerId { get; set; }
        public Traveler Traveler { get; set; }


        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }




    }
}
