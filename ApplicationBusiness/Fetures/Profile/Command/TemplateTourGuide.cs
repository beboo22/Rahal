using ApplicationBusiness.Dtos.Auth;
using ApplicationBusiness.Dtos.Profile;
using ApplicationBusiness.Fetures.BookingTripService.Query.Response;
using ApplicationBusiness.Fetures.PostService.Query.Response;
using ApplicationBusiness.Fetures.TripService.Query.Response;

namespace ApplicationBusiness.Fetures.Profile.Command
{
    public class TemplateTourGuide 
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public string PhotoUrl { get; internal set; }

        public int NumberFollowing { get; set; }
        public int NumberFollowers { get; set; }

        public int? Id { get; set; }
        public string Ssn { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public List<BusinessGalaryDto> BusinessGalaries { get; set; }
        public List<ExperiencePostTemplate> ExperiencePostTemplates { get; set; }
        public List<PrivateTemplateTrip> PrivateTrips { get; set; }
        public List<BookingTripTemplate> BookedTrip { get; set; }
        public decimal SalaryPerDay { get; internal set; }

    }
    public class TemplateTokenTour
    {
        public Token Token { get; set; }

        public TemplateTourGuide profile { get; internal set; }



    }
    public class TemplateTourSearch
    {
        public string? Photo { get; internal set; }
        public string Email { get; internal set; }
        public string name { get; internal set; }
        public int Id { get; internal set; }
    }





}
