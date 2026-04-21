using Domain.Entity.Identity;

namespace ApplicationBusiness.Services
{
    public class Order
    {
        public string ProviderRef {  get; set; }
        public decimal TotalBookingPrice { get; set; }
        public string ItemDesc { get; internal set; }
        public User? User { get; internal set; }
    }
}