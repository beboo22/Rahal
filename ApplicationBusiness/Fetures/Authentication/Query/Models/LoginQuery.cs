using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Auth;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Fetures.Authentication.Query.Models
{
    public record LoginQuery(LoginDto loginDto) : IQuery<ApiResponse>;
}
