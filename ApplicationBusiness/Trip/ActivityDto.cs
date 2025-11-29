using Domain.Entity.TripEntity;

namespace ApplicationBusiness.Dtos.Trip
{
    public class ActivityDto
    {
        public string Destination { get; set; } = null!;
        public string Title { get; set; } = null!;
        public decimal FullPrice { get; set; }
        public int SelectedDay { get; set; }
        public string Image { get; set; }
        public TripCategory TripCategory { get; set; }
        public TimeOnly StartAt { get; set; }
        public TimeOnly EndAt { get; set; }
    }
}