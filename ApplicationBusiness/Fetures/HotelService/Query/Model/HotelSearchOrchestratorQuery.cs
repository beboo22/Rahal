using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Hotels;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.HotelService.Query.Model
{
    public record HotelSearchOrchestratorQuery(
    HotelSearchRequest Request
) : IQuery<ApiResponse>;

    public record GetHotelsQuery(
    HotelSearchRequest Request
) : IQuery<ApiResponse>;
}
