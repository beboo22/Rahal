using ApplicationBusiness.Dtos.Trip;
using ApplicationBusiness.Fetures.TripService.Command;
using ApplicationBusiness.Fetures.TripService.Command.Models;
using ApplicationBusiness.Fetures.TripService.Query;
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

    public class ReviewsController : ApiController
    {
        public ReviewsController(ISender sender) : base(sender)
        {
        }

        /// <summary>
        /// Add Review To Public Trip
        /// </summary>
        [HttpPost("PublicTrip")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddPublicTripReview([FromForm] AddTripReviewDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var result = await Sender.Send(
                new AddReviewToPubliucTrip(dto, userId.Value));

            return ProcessResult(result);
        }

        /// <summary>
        /// Add Review To Private Trip
        /// </summary>
        //[HttpPost("PrivateTrip")]
        //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> AddPrivateTripReview([FromForm] AddTripReviewDto dto)
        //{
        //    var userId = GetUserId();
        //    if (userId == null)
        //        return Unauthorized();

        //    var result = await Sender.Send(
        //        new AddReviewToPrivateTrip(dto, userId.Value));

        //    return ProcessResult(result);
        //}

        [HttpPost("TourGuide")]
        public async Task<IActionResult> AddTourGuideReview(
    [FromForm] AddReviewTourGuideDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var result = await Sender.Send(
                new AddReviewToTourGuide(dto, userId.Value));

            return ProcessResult(result);
        }

        [HttpPost("Hotel")]
        public async Task<IActionResult> AddHotelReview(
            [FromForm] AddReviewHotelDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var result = await Sender.Send(
                new AddReviewToHotel(dto, userId.Value));

            return ProcessResult(result);
        }
        [HttpGet("PublicTrip/{tripId}")]
        public async Task<IActionResult> GetPublicTripReviews(int tripId)
        {
            var result = await Sender.Send(
                new GetPublicTripReviews(tripId));

            return ProcessResult(result);
        }

        //[HttpGet("PrivateTrip/{tripId}")]
        //public async Task<IActionResult> GetPrivateTripReviews(int tripId)
        //{
        //    var result = await Sender.Send(
        //        new GetPrivateTripReviews(tripId));

        //    return ProcessResult(result);
        //}

        [HttpGet("TourGuide/{tourGuideId}")]
        public async Task<IActionResult> GetTourGuideReviews(int tourGuideId)
        {
            var result = await Sender.Send(
                new GetTourGuideReviews(tourGuideId));

            return ProcessResult(result);
        }

        [HttpGet("Hotel/{hotelId}")]
        public async Task<IActionResult> GetHotelReviews(int hotelId)
        {
            var result = await Sender.Send(
                new GetHotelReviews(hotelId));

            return ProcessResult(result);
        }

    }
}
