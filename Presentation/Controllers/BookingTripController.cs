using ApplicationBusiness.Fetures.BookingTripService.Command.Models;
using ApplicationBusiness.Fetures.BookingTripService.Query.Models;
using ApplicationBusiness.Fetures.BookingTripService.Query.Response;
using ApplicationBusiness.Fetures.TripService.Query.Models;
using ApplicationBusiness.Fetures.TripService.Query.Response;
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
    public class BookingTripController : ApiController
    {
        public BookingTripController(ISender sender): base(sender)
        {
        }

        // GET: api/<BookingTripController>
        [ProducesResponseType(typeof(ApiResultResponse<List<BookingTripTemplate>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var res = await Sender.Send(new GetAllBooking());
            return Ok(res);
        }
        // GET api/<BookingTripController>/5
        [ProducesResponseType(typeof(ApiResultResponse<BookingTripTemplate>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var res = await Sender.Send(new GetBookingById(id));
            return Ok(res);
        }
        // POST api/<BookingTripController>//UserId/TripId
        /// <summary>
        /// Creates a new trip.
        /// </summary>
        /// <response code="201">Returns the created trip ID and message</response>
        [ProducesResponseType(typeof(ApiResultResponse<BookingTripTemplate>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TourGuide,Traveler")]

        public async Task<IActionResult> Post(int UserId, int TripId)
        {
            var result = await Sender.Send(new BookTrip(UserId, TripId));
            return Ok(result);
        }
        [ProducesResponseType(typeof(ApiResultResponse<BookingTripTemplate>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpDelete("cancel")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TourGuide,Traveler")]

        public async Task<IActionResult> Delete(int BookingId)
        {
            var result = await Sender.Send(new DeleteBookTrip(BookingId));
            return Ok(result);
        }
    }
}
