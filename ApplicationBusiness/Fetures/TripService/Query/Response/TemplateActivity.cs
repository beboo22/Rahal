using Domain.Entity.TripEntity;

namespace ApplicationBusiness.Fetures.TripService.Query.Response
{
    public class TemplateActivity
    {
        public int Id { get; set; }
        public string Destination { get; set; }
        public decimal FullPrice { get; set; }
        public int SelectedDay { get; set; }
        public TimeOnly EndAt { get; set; }
        public string Image { get; set; }
        public TimeOnly StartAt { get; set; }
        public string Title { get; set; }
        public TripCategory TripCategory { get; set; }
        public string? PlaceId { get; set; }
        public string? DataId { get; set; }
        public string? ActivityType { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Address { get; set; }
        public string? Website { get; set; }
        public string? Phone { get; set; }
        public double? Rating { get; set; }
        public int? Reviews { get; set; }
        public string? PriceRange { get; set; }
        public string? Description { get; set; }
        public List<string> serviceOption { get; set; }
    }
}