using ApplicationBusiness.Fetures.Profile.Command;

namespace ApplicationBusiness.Fetures.Authentication.Query
{
    public class TemplateGenericProfile
    {
        public List<string> Roles { get; set; }
        public int Id { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Email { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }
        
        public TemplateTraveler? Traveler { get; set; }
        public TemplateTourGuide? TourGuide { get; set; }
        public TemplateTravelComapny? TravelCompany { get; set; }
    }
}