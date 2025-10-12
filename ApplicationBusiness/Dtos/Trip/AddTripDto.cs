using Domain.Entity.TripEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Dtos.Trip
{
    public class AddPrivateTripDto
    {
        public string From { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public string Title { get; set; } = null!;
        //public int? TourGuideId { get; set; }
        public TripCategory TripCategory { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public ICollection<ActivityDto> Activities { get; set; } = new List<ActivityDto>();

    }
    public class AddPublicTripDto
    {
        public string From { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public string Title { get; set; } = null!;
        //public decimal Price { get; set; }
        public int IncludedPackages { get; set; }
        public TripCategory TripCategory { get; set; }
        public int NumberOfMember { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public int? TourGuideId { get; set; }
        public ICollection<ActivityDto> Activities { get; set; } = new List<ActivityDto>();

    }
}
