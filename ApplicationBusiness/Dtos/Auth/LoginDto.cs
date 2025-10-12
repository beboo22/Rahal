using Domain.Entity.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Dtos.Auth
{
    public class LoginDto
    {
        public string Password { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
