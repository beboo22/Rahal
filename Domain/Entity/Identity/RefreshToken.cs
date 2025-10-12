using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.Identity
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool Revoked { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
