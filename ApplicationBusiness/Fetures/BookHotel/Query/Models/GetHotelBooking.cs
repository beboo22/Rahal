using Application.Abstraction.message;
using ApplicationBusiness.Abstraction.spacification;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.BookHotel.Query.Models
{
    public record GetHotelBooking(PaymentFilter Filter) :IQuery<ApiResponse>;
}
