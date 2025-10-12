using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Auth;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ApplicationBusiness.Fetures.Authentication.Command.Models
{
    public record ResetPasswordCommand(ResetPasswordDto ResetPasswordDto):ICommand<ApiResponse>;
}
