using ApplicationBusiness.Fetures.BookingTripService.Command.Models;
using ApplicationBusiness.Fetures.BookingTripService.Query.Models;
using ApplicationBusiness.Fetures.BookingTripService.Query.Response;
using Domain.BaseResponce;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Traveler,TourGuide,TravelerProfileController")]

    public class BookingPrivTripController : ApiController
    {
        public BookingPrivTripController(ISender sender) : base(sender) { }

        // GET: api/BookingTrip
        [ProducesResponseType(typeof(ApiResultResponse<List<BookingTripTemplate>>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var res = await Sender.Send(new GetAllBooking());
            return ProcessResult(res);
        }

        // GET: api/BookingTrip/5
        [ProducesResponseType(typeof(ApiResultResponse<BookingTripTemplate>), StatusCodes.Status200OK)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var res = await Sender.Send(new GetBookingById(id));
            return ProcessResult(res);
        }

        /// <summary>
        /// Books a new trip for the current user.
        /// </summary>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TourGuide,Traveler")]
        [ProducesResponseType(typeof(ApiResultResponse<BookingTripTemplate>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(int TripId, int? HotelId, int? flightId)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse(401));

            // Use userId from the token instead of the parameter for better security
            var result = await Sender.Send(new BookPrivTrip(userId.Value, TripId, HotelId, flightId));

            return ProcessResult(result);
        }

        /// <summary>
        /// Cancels/Deletes a trip booking.
        /// </summary>
        [HttpDelete("cancel")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TourGuide,Traveler")]
        [ProducesResponseType(typeof(ApiResultResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int BookingId)
        {
            var result = await Sender.Send(new DeletePrivBookTrip(BookingId));
            return ProcessResult(result);
        }
    }

}
