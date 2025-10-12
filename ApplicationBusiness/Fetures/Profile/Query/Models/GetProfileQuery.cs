using Application.Abstraction.message;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Profile.Query.Models
{
    public record GetTravelerCompanyProfileQuery(int UserId):IQuery<ApiResponse>;
    public record GetTravelerProfileQuery(int UserId):IQuery<ApiResponse>;
    public record GetTourGuideProfileQuery(int UserId):IQuery<ApiResponse>;
}
