using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Hotels;
using ApplicationBusiness.Dtos.Photos;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PhotoService.Query.Model
{

    public record PhotoSearchOrchestratorQuery(
    SearchPhotoReq Request
) : IQuery<ApiResponse>;
    public record GetPhotoQuery(SearchPhotoReq Req) :IQuery<ApiResponse>;
}
