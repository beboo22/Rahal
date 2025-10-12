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
    }
}