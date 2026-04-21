using Domain.Entity.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.TripEntity
{
    [NotMapped]
    public abstract class Trip : BaseEntity
    {
        public string From { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public string Title { get; set; } = null!;
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public TripCategory TripCategory { get; set; }
        public int CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public User CreatedBy { get; set; }

    }
    public class PrivateTrip : Trip
    {
        public ICollection<Review> Reviews { get; set; }
        public int? TourGuideId { get; set; }
        [ForeignKey("TourGuideId")]
        public User? TourGuide { get; set; }
        // Additional service fee for personalization
        public decimal? CustomizationFee { get; set; }
        public ICollection<ActivityPrivateTrip> PrivateActivities { get; set; } = new List<ActivityPrivateTrip>();
        public ICollection<RequestTourGuidePrivateTrip> requestTourGuides { get; set; }
         = new List<RequestTourGuidePrivateTrip>();
        public ICollection<BookingPrivateTrip> BookingPrivateTrips { get; set; }
    }
    public class PublicTrip : Trip
    {
        public ICollection<Review> Reviews { get; set; }
        public ICollection<RequestTourGuidePulicTrip> requestTourGuides { get; set; }= new List<RequestTourGuidePulicTrip>();
        public ICollection<ActivityPublicTrip> PublicActivities { get; set; } = new List<ActivityPublicTrip>();
        public Package IncludedPackages { get; set; }
        public int MaxNumberOfMember { get; set; }
        public int CurrentNumberOfMember { get; set; }

        public decimal TravelerFee { get; set; }
        public decimal OwnerTripFee { get; set; }

        public int? TourGuideId { get; set; }
        [ForeignKey("TourGuideId")]
        public User? TourGuide { get; set; }
        public ICollection<BookingPublicTrip> BookingPublicTrips { get; set; }
    }


}
