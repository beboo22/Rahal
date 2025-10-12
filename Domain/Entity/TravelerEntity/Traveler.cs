using Domain.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.TravelerEntity
{
    public class Traveler:BaseEntity
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
        public ICollection<TrvelerAddress> trvelerAddresses { get; set; }

      
    }
}
