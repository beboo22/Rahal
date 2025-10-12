using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Authentication.Command.Models
{
    public record RequestVerificationCommand(string Email,string frontPhoto,string backPhoto,string userWithIdenty);
}
