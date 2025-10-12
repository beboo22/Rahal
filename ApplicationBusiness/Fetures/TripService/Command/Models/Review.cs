using Application.Abstraction.message;
using ApplicationBusiness.Dtos.Trip;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ApplicationBusiness.Fetures.TripService.Command.Models
{
    public record AddReviewToPubliucTrip(AddTripReviewDto dto, int UserId):ICommand<ApiResponse>;
    public record AddReviewToPrivateTrip(AddTripReviewDto dto, int UserId):ICommand<ApiResponse>;
}
