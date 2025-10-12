using Domain.Entity.Identity;
using Domain.Entity.TripEntity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationBusiness.Fetures.BookingTripService.Query.Response
{
    public class BookingTripTemplate
    {
        public string TripTilte { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public decimal TotalBookingPrice { get; set; }
        public bool IsPaid { get; set; } = false;
        public int Id { get; internal set; }
    }
}