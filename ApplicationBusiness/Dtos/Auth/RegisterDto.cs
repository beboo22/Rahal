using Domain.Entity.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Dtos.Auth
{
    public class RegisterDto
    {
        [Required]
        public string FName { get; set; } = null!;
        [Required]
        public string LName { get; set; } = null!;
        [Range(18, 70)]
        public int Age { get; set; }

        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        public virtual List<LanguagesDto> Languages { get; set; }
        public virtual List<PhoneNumberDto> phoneNumbers { get; set; }
        public List<int> RoleIds { get; set; }


    }

    public class PhoneNumberDto
    {
        public bool HasWhatsApp { get; set; }
        public string CountryCode { get; set; }
        public string Number { get; set; }
    }

    public class LanguagesDto
    {
        public string Code { get; set; }
    }
}
