using ApplicationBusiness.Dtos.Profile;
using ApplicationBusiness.Fetures.BookingTripService.Query.Response;
using ApplicationBusiness.Fetures.PostService.Query.Response;
using ApplicationBusiness.Fetures.TripService.Query.Response;

namespace ApplicationBusiness.Fetures.Profile.Command
{
    public class TemplateTourGuide 
    {
        public string PhotoUrl { get; internal set; }

        public int? Id { get; set; }
        public string Ssn { get; set; }
        public string Bio { get; set; }
        public List<BusinessGalaryDto> BusinessGalaries { get; set; }
        public List<Adress> Adresses { get; set; }
        public List<ExperiencePostTemplate> ExperiencePostTemplates { get; set; }
        public List<PrivateTemplateTrip> PrivateTrips { get; set; }
        public List<BookingTripTemplate> BookedTrip { get; set; }
        public decimal SalaryPerDay { get; internal set; }

    }
}