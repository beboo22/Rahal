using Domain.Entity.Identity;
using Domain.Entity.TourGuidEntity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.TripEntity
{
    public class RequestTourGuidePulicTrip : BaseEntity
    {
        public int PublicTripId { get; set; }
        [ForeignKey(nameof(PublicTripId))]
        public PublicTrip PublicTrip { get; set; }
        public int TourGuideId { get; set; }
        [ForeignKey(nameof(TourGuideId))]
        public TourGuide TourGuide { get; set; }
    }
    public class RequestTourGuidePrivateTrip : BaseEntity
    {
        public int PrivateTripId { get; set; }
        [ForeignKey(nameof(PrivateTripId))]
        public PrivateTrip PrivateTrip { get; set; }
        public int TourGuideId { get; set; }
        [ForeignKey(nameof(TourGuideId))]
        public TourGuide TourGuide { get; set; }
    }
}