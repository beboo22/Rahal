using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Trip;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.TripService.Command.Models
{
    public record AddPrivateTrip(AddPrivateTripDto dto,int CreatedById):ICommand<ApiResponse>;
}
