using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Auth;
using Domain.BaseResponce;
using Domain.Entity;
using Domain.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Fetures.Authentication.Command.Models
{
    public  record signUpCommand(RegisterDto signUpData) :ICommand<ApiResponse>;
}
