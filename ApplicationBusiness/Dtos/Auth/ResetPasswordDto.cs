using Domain.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Dtos.Auth
{
    public class ResetPasswordDto
    {
        public string NewPassword { get; set; }
        public string Token { get; set; }
    }
}
