using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Fetures.BookHotel.Command.Models;
using ApplicationBusiness.Fetures.BookHotel.Query.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookHotelController : ApiController
    {
        public BookHotelController(ISender sender) : base(sender) { }





        [HttpPost()]
        public async Task<IActionResult> BookHotels(int hotelId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized();

            var response = await Sender.Send(new BookHotelcommand(userId.Value, hotelId));

            return StatusCode(response.statusCode, response);
        }
        [HttpGet("GetBy")]
        public async Task<IActionResult> GetBookHotelsBY([FromQuery]PaymentFilter filter)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized();
            filter.UserId = userId.Value;

            var response = await Sender.Send(new GetHotelBooking(filter));

            return StatusCode(response.statusCode, response);
        }


        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out int id) ? id : (int?)null;
        }


    }
}
