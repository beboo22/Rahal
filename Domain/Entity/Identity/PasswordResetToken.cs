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
    public class PasswordResetToken : BaseEntity
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
