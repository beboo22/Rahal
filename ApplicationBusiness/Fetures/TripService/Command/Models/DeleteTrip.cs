using Application.Abstraction.message;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ApplicationBusiness.Fetures.TripService.Command.Models
{
    public record DeletePublicTrip(int Id, int createdBy, List<RoleEnum> roles) :ICommand<ApiResponse>;
    public record DeletePrivateTrip(int Id,int createdBy):ICommand<ApiResponse>;
}
