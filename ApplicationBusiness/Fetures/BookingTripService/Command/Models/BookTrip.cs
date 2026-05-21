using Application.Abstraction.message;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.BookingTripService.Command.Models
{
    public record BookTrip(int UserId, int TripId,int? HotelId,int? flightId) : ICommand<ApiResponse>;
    public record BookPrivTrip(int UserId, int TripId,int? HotelId,int? flightId) : ICommand<ApiResponse>;
    public record DeleteBookTrip(int BookingId) : ICommand<ApiResponse>;
    public record DeletePrivBookTrip(int BookingId) : ICommand<ApiResponse>;
    public record ReturnMonyToUser(int TripId) : IQuery<ApiResponse>;
    public record ReturnPrivMonyToUser(int TripId) : IQuery<ApiResponse>;

}
