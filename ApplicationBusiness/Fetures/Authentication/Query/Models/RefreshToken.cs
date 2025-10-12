using Application.Abstraction.message;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Authentication.Query.Models
{
    public record  RefreshTokenModel(string refreshtoken):IQuery<ApiResponse>;
}
