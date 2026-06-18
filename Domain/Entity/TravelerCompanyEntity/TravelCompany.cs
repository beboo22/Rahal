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
        public string? PhotoUrl { get; set; }

        //public decimal? TotalEarnings { get; set; } = 0;


        #region verification
        public string? FrontIdentityPhotoUrl { get; set; } = null!;
        public string? BackIdentityPhotoUrl { get; set; } = null!;
        public string? Bio { get; set; } = null!;
        public string? Ssn { get; set; } = null!;
        #endregion
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public ICollection<HiringPost> HiringPosts { get; set; } = new List<HiringPost>();
        public ICollection<TravelCompanyBusinessGalary> TravelCompanyBusinessGalaries { get; set; } = new List<TravelCompanyBusinessGalary>();

    }
}
