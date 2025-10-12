using ApplicationBusiness.Dtos.Profile;

namespace ApplicationBusiness.Fetures.Profile.Command
{
    public class TemplateTraveler
    {
        public string PhotoUrl { get; internal set; }


        public string Ssn { get; internal set; }
        public string Bio { get; internal set; }
        public string? FrontIdentityPhotoUrl { get; internal set; }
        public string? BackIdentityPhotoUrl { get; internal set; }
        public List<Adress> Adresses { get; internal set; }
        public int? Id { get; internal set; }
    }
    public class TemplateTravelComapny
    {
        public string PhotoUrl { get; internal set; }

        public int? Id { get; internal set; }
        public string Ssn { get; internal set; }
        public string Bio { get; internal set; }
        public string? FrontIdentityPhotoUrl { get; internal set; }
        public string? BackIdentityPhotoUrl { get; internal set; }
        public List<BusinessGalaryDto> BusinessGalaries { get; internal set; }
        public List<Adress> Adresses { get; internal set; }
    }
}