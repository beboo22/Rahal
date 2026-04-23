using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Hotels;
using ApplicationBusiness.Dtos.Photos;
using Domain.BaseResponce;
using Domain.Entity.photo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PhotoService.Command.Model
{

    public record SavePhotoCommand(
    PhotoSearchResponse Response,
        string exactKey,
        string exactKeyOrgin,
        string groupKey
) : ICommand<ApiResponse>;
}
