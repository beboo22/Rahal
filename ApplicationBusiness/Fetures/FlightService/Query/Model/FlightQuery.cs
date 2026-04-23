using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Flights;
using ApplicationBusiness.Dtos.Hotels;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.FlightService.Query.Model
{
    public record FlightQuery(FlightSearchRequest filter) :IQuery<ApiResponse>;
    public record SearchFlightOrchestratorQuery(FlightSearchRequest Filter) : IQuery<ApiResponse>;
    public record GetFlightQuery(FlightSearchRequest Filter) : IQuery<ApiResponse>;
}
