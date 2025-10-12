using ApplicationBusiness.Dtos.Profile;

namespace ApplicationBusiness.Fetures.Profile.Command
{
    public class TemplateTourGuide 
    {
        public string PhotoUrl { get; internal set; }

        public int? Id { get; set; }
        public string Ssn { get; set; }
        public string Bio { get; set; }
        public string? FrontIdentityPhotoUrl { get; set; }
        public string? BackIdentityPhotoUrl { get; set; }
        public List<BusinessGalaryDto> BusinessGalaries { get; set; }
        public List<Adress> Adresses { get; set; }
        public decimal SalaryPerDay { get; internal set; }
    }
}