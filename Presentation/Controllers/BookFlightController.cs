using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Fetures.BookingFlight.Command;
using ApplicationBusiness.Fetures.BookingFlight.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookFlightController : ApiController
    {
        public BookFlightController(ISender sender) : base(sender) { }

        [HttpPost]
        public async Task<IActionResult> BookFlight(int flightOfferId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized();

            var response = await Sender.Send(new BookFlightCommand(userId.Value, flightOfferId));

            return StatusCode(response.statusCode, response);
        }

        [HttpGet("GetBy")]
        public async Task<IActionResult> GetBookFlightsBy([FromQuery]PaymentFilter filter)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized();

            filter.UserId = userId.Value;

            var response = await Sender.Send(new GetFlightBookingQuery(filter));

            return StatusCode(response.statusCode, response);
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out int id) ? id : (int?)null;
        }
    }
}
