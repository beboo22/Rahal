using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.Identity
{
    public class PhoneNumber:BaseEntity
    {
        public bool HasWhatsApp { get; set; }
        public string CountryCode { get; set; }
        public string Number { get; set; }


        public int UserId { get; set; }
        public User User { get; set; }
    }
}
