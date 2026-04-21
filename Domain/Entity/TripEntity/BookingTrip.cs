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
    public abstract class Booking : BaseEntity
    {

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public decimal TotalBookingPrice { get; set; }
        public decimal TotalOwnerProfit { get; set; }
        public decimal AppProfit { get; set; }
        public bool IsPaid { get; set; } = false;
        public bool Canceled { get; set; } = false;
    }



    public class BookingPublicTrip : Booking
    {
        public int PublicTripId { get; set; }
        [ForeignKey(nameof(PublicTripId))]
        public PublicTrip PublicTrip { get; set; }


    }
    public class BookingPrivateTrip : Booking
    {
        public int PrivateTripId { get; set; }
        [ForeignKey(nameof(PrivateTripId))]
        public PrivateTrip PrivateTrip { get; set; }


    }



}
