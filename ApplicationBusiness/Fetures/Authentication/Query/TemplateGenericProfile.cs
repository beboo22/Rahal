using ApplicationBusiness.Fetures.Profile.Command;

namespace ApplicationBusiness.Fetures.Authentication.Query
{
    public class TemplateGenericProfile
    {
        public string Fname { get; set; }
        public string Lname { get; set; }
        public TemplateTraveler? Traveler { get; set; }
        public TemplateTourGuide? TourGuide { get; set; }
        public TemplateTravelComapny? TravelCompany { get; set; }
    }
}