using ApplicationBusiness.Dtos.Trip;
using ApplicationBusiness.Fetures.TripService.Command.Models;
using ApplicationBusiness.Fetures.TripService.Query.Models;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.BaseResponce;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicTripController : ApiController
    {
        public PublicTripController(ISender sender) : base(sender) { }

        /// <summary>
        /// Creates a new public trip.
        /// </summary>
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTrip>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany,TourGuide,Traveler")]
        [HttpPost()]
        public async Task<IActionResult> CreatePublicTrip([FromBody] AddPublicTripDto dto)
        {
            var userId = GetUserId();

            var result = await Sender.Send(new AddPublicTrip(dto, userId.Value));
            return Ok(result);
        }
        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out int id) ? id : (int?)null;
        }
        /// <summary>
        /// Delete a public trip.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePublicTrip(int id)
        {
            var result = await Sender.Send(new DeletePublicTrip(id));
            return Ok(result);
        }

        /// <summary>
        /// Get all public trips.
        /// </summary>
        [ProducesResponseType(typeof(ApiResultResponse<List<TemplateTrip>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany,TourGuide,Traveler")]
        [HttpGet("All")]
        public async Task<IActionResult> GetAllTrips()
        {
            var result = await Sender.Send(new GetAllTrip());
            return Ok(result);
        }

        /// <summary>
        /// Search for public trips.
        /// </summary>
        [ProducesResponseType(typeof(ApiResultResponse<List<TemplateTrip>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpPost("Search")]
        public async Task<IActionResult> SearchForTrip([FromBody] SearchForTripDto dto)
        {
            var result = await Sender.Send(new SearchForTrip(dto));
            return Ok(result);
        }
    }
    [Route("api/[controller]")]
    [ApiController]

    public class PrivateTripController : ApiController
    {
        public PrivateTripController(ISender sender) : base(sender) { }

        /// <summary>
        /// Creates a new private trip.
        /// </summary>
        [ProducesResponseType(typeof(ApiResultResponse<PrivateTemplateTrip>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany,TourGuide,Traveler")]
        [HttpPost()]
        public async Task<IActionResult> CreatePrivateTrip([FromBody] AddPrivateTripDto dto)
        {
            var userId = GetUserId();

            var result = await Sender.Send(new AddPrivateTrip(dto, userId.Value));
            return Ok(result);
        }
        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out int id) ? id : (int?)null;
        }
        /// <summary>
        /// Delete a private trip.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePrivateTrip(int id)
        {
            var result = await Sender.Send(new DeletePrivateTrip(id));
            return Ok(result);
        }

        /// <summary>
        /// Get private trips for a specific user.
        /// </summary>
        [ProducesResponseType(typeof(ApiResultResponse<List<PrivateTemplateTrip>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany,TourGuide,Traveler")]
        [HttpGet("User")]
        public async Task<IActionResult> GetPrivateTripsByUserId()
        {
            var userId = GetUserId();

            var result = await Sender.Send(new GetPrivateTripforUserId(userId.Value));
            return Ok(result);
        }
    }
}
