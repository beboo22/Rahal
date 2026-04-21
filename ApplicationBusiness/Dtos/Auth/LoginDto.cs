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
        [EmailAddress]
        public string Email { get; set; }
    }
}
