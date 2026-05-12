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

        // البيانات اللي جاية من الـ Front-end بعد اختيار الـ Option
        public string? PlaceId { get; set; }
        public string? DataId { get; set; }
        public string? ActivityType { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Address { get; set; }
        public string? Thumbnail { get; set; }
        public string? Website { get; set; }
        public string? Phone { get; set; }
        public double? Rating { get; set; }
        public int? Reviews { get; set; }
        public string? PriceRange { get; set; }
        public string? Description { get; set; }
        public List<string> serviceOption { get; set; }

    }
}