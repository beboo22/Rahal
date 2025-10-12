using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.TourGuidEntity
{
    public class TourGuideAddress
    {
        public int Id { get; set; }
        public int TourGuideId { get; set; }
        public TourGuide TourGuide { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
    }
}
