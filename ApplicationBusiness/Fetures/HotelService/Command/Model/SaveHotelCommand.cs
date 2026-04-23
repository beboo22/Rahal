using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Hotels;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.HotelService.Command.Model
{
    public record SaveHotelCommand(
    HotelSearchResponse Response,
        string exactKey,
    string groupKey
) : ICommand<ApiResponse>;
}
