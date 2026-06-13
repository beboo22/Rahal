using Domain.Entity.Hotel_flights;
using Domain.Entity.Identity;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.TripEntity
{
    public abstract class Review:BaseEntity
    {
        

        public int? ReviewerId { get; set; }
        public User? Reviewer { get; set; }

        //public int? RevieweeId { get; set; }
        //public User? Reviewee { get; set; }
        
        public decimal Rating { get; set; }
        public string Feedback { get; set; }
    }
    public class ReviewPublicTrip : Review
    {
        public int PublicTripId { get; set; }
        public PublicTrip PublicTrip { get; set; }
    }
    public class ReviewPrivateTrip : Review
    {
       

        public int PrivateTripId { get; set; }
        public PrivateTrip PrivateTrip { get; set; }

        
    }
    public class ReviewTourGuide : Review
    {
       

        public int TourGuideId { get; set; }
        public TourGuide TourGuide { get; set; }

        
    }
    public class ReviewHotel : Review
    {
       

        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }

        
    }




    
}
