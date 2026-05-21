using Application.Abstraction.message;
using Domain.BaseResponce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.BookHotel.Command.Models
{
    public record BookHotelcommand(int UserId,int HotleId, int durationInDay=1) :ICommand<ApiResponse>;
}
