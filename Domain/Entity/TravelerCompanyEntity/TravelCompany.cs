using Domain.Entity.Identity;
using Domain.Entity.PostEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.TravelerCompanyEntity
{
    public class TravelCompany:BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public string PhotoUrl { get; set; }


        #region verification
        public string? FrontIdentityPhotoUrl { get; set; } = null!;
        public string? BackIdentityPhotoUrl { get; set; } = null!;
        public string? Bio { get; set; } = null!;
        public string? Ssn { get; set; } = null!;
        #endregion

        public ICollection<TravelerCompanyAddress> traveleCompanyAddresses {  get; set; } = new List<TravelerCompanyAddress>();
        public ICollection<TravelCompanyBusinessGalary> travelCompanyBusinessGalaries { get; set; } = new List<TravelCompanyBusinessGalary>();
        public ICollection<HiringPost> HiringPosts { get; set; } = new List<HiringPost>();
       
    }
}
