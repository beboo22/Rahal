using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Fetures.BookingFlight.Command;
using ApplicationBusiness.Fetures.BookingFlight.Query;
using Domain.BaseResponce;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Books a flight for the current user.
        /// </summary>
        /// <param name="flightOfferId">The ID of the flight offer to book.</param>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResultResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> BookFlight(int flightOfferId)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse(401, "User ID claim missing"));

            var response = await Sender.Send(new BookFlightCommand(userId.Value, flightOfferId));

            return ProcessResult(response);
        }

        /// <summary>
        /// Retrieves flight bookings based on the provided filters.
        /// </summary>
        [HttpGet("GetBy")]
        [ProducesResponseType(typeof(ApiResultResponse<List<object>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBookFlightsBy([FromQuery] PaymentFilter filter)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse(401, "User ID claim missing"));

            filter.UserId = userId.Value;

            var response = await Sender.Send(new GetFlightBookingQuery(filter));

            return ProcessResult(response);
        }
    }

}
