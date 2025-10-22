using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Trip;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.TripService.Query.Models
{
    public record GetPrivateTripforUserId(int id):IQuery<ApiResponse>;
    public record SearchForTrip(SearchForTripDto dto):IQuery<ApiResponse>;
}
