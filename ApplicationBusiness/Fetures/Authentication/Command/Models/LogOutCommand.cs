using Application.Abstraction.message;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Authentication.Command.Models
{
    public record LogOutCommand(string RefreshToken) : ICommand<ApiResponse>;
}
