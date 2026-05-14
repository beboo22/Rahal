using ApplicationBusiness.Dtos.Auth;
using ApplicationBusiness.Dtos.Profile;
using ApplicationBusiness.Fetures.BookingTripService.Query.Response;
using ApplicationBusiness.Fetures.PostService.Query.Response;
using ApplicationBusiness.Fetures.TripService.Query.Response;

namespace ApplicationBusiness.Fetures.Profile.Command
{
    public class TemplateTraveler
    {
        public string PhotoUrl { get; internal set; }
        public int NumberFollowing { get; set; }
        public int NumberFollowers { get; set; }
        public string Email { get; internal set; }

        public List<ExperiencePostTemplate> ExperiencePostTemplates { get; set; }
        public List<PrivateTemplateTrip> PrivateTrips { get; set; }
        public List<BookingTripTemplate> BookedTrip { get; set; }
        public string Ssn { get; internal set; }
        public string Bio { get; internal set; }
        //public List<Adress>? Adresses { get; internal set; }

        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }


        public int? Id { get; internal set; }
    }
    public class TemplateTokenTraveler
    {
        public Token Token { get; set; }

        public TemplateTraveler profile { get; internal set; }


        
    }
    public class TemplateTravelComapny
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public int NumberFollowing { get; set; }
        public int NumberFollowers { get; set; }
        public string PhotoUrl { get; internal set; }

        public List<ExperiencePostTemplate> ExperiencePostTemplates { get; set; }
        public List<PrivateTemplateTrip> PrivateTrips { get; set; }
        public List<BookingTripTemplate> BookedTrip { get; set; }
        public int? Id { get; internal set; }
        public string Ssn { get; internal set; }
        public string Bio { get; internal set; }
        public List<BusinessGalaryDto> BusinessGalaries { get; internal set; }
        //public List<Adress> Adresses { get; internal set; }
        public string Email { get;  set; }
    }
    
}