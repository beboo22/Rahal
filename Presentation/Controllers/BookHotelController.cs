using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Fetures.BookHotel.Command.Models;
using ApplicationBusiness.Fetures.BookHotel.Query.Models;
using Domain.BaseResponce;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Books a hotel for the current user.
        /// </summary>
        /// <param name="hotelId">The ID of the hotel to book.</param>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResultResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> BookHotels(int hotelId,int durationInDay)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse(401, "User ID claim missing"));

            var response = await Sender.Send(new BookHotelcommand(userId.Value, hotelId,durationInDay));

            return ProcessResult(response);
        }

        /// <summary>
        /// Retrieves hotel bookings based on filters.
        /// </summary>
        [HttpGet("GetBy")]
        [ProducesResponseType(typeof(ApiResultResponse<List<object>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBookHotelsBY([FromQuery] PaymentFilter filter)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse(401, "User ID claim missing"));

            filter.UserId = userId.Value;

            var response = await Sender.Send(new GetHotelBooking(filter));

            return ProcessResult(response);
        }
    }
}
