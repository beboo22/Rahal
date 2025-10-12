using Application.Abstraction.message;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.BookingTripService.Command.Models
{
    public record BookTrip(int UserId, int TripId) : ICommand<ApiResponse>;
    public record DeleteBookTrip(int BookingId) : ICommand<ApiResponse>;
}
