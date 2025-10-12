using Application.Abstraction.message;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.BookingTripService.Query.Models
{
    public record GetAllBooking:IQuery<ApiResponse>;
}
