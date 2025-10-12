using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Dtos.Trip
{
    public class AddTripReviewDto
    {

        public int? TourguideId { get; set; }
        public int? TravelCompanyId { get; set; }
        public int TripId { get; set; }
        public string Feedback { get; set; }
        [Range(0, 5)]
        public decimal Rate { get; set; }
    }
}
