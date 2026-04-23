using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Dtos.Flights;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.FlightService.Command.Model
{
    public record FlightCommand(FlightSearchResponse res,string cacheKey) :ICommand<ApiResponse>;
    public record SaveFlightCommand(
    FlightSearchResponse Response,
    string exactKey,
    string groupKey
        ) : ICommand<ApiResponse>;
}
