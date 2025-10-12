using Domain.Entity.Identity;
using Domain.Entity.TravelerEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.TripEntity
{
    public class Review:BaseEntity
    {
        public int? PublicTripId { get; set; }
        public PublicTrip? PublicTrip { get; set; }

        public int? PrivateTripId { get; set; }
        public PrivateTrip? PrivateTrip { get; set; }

        public int? ReviewerId { get; set; }
        public User? Reviewer { get; set; }

        public int? RevieweeId { get; set; }
        public User? Reviewee { get; set; }
        
        public decimal Rating { get; set; }
        public string Feedback { get; set; }
    }
    
}
