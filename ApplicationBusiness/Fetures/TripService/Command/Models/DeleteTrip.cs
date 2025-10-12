using Application.Abstraction.message;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ApplicationBusiness.Fetures.TripService.Command.Models
{
    public record DeletePublicTrip(int Id):ICommand<ApiResponse>;
    public record DeletePrivateTrip(int Id):ICommand<ApiResponse>;
}
