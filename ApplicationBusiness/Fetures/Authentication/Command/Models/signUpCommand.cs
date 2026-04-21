using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Auth;
using Domain.BaseResponce;
using Domain.Entity;
using Domain.Entity.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Fetures.Authentication.Command.Models
{
    public  record signUpCommand(RegisterDto signUpData) :ICommand<ApiResponse>;
    public  record IsUserExist(int? UserId) :ICommand<ApiResponse>;
    public record VerifiedUser(int Id):ICommand<ApiResponse>;
    public record VerifyOtpCommand : ICommand<ApiResponse>
    {
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string Otp { get; set; } = null!;
    }
}
