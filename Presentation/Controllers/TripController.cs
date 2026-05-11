using ApplicationBusiness.Abstraction.spacification;
using ApplicationBusiness.Dtos.Trip;
using ApplicationBusiness.Fetures.TripService.Command.Models;
using ApplicationBusiness.Fetures.TripService.Query.Models;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.BaseResponce;
using Domain.Entity.Identity;
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
        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePublicTrip(int id)
        {
            var roleClaims = User.FindAll(ClaimTypes.Role).Select(c => c.Value);

            // 2. Parse the strings into a List<RoleEnum>
            var roles = new List<RoleEnum>();
            foreach (var roleValue in roleClaims)
            {
                if (Enum.TryParse<RoleEnum>(roleValue, out var role))
                {
                    roles.Add(role);
                }
            }

            // 3. Send the command
            var result = await Sender.Send(new DeletePublicTrip(id,GetUserId().Value,roles));
            return Ok(result);
        }

        /// <summary>
        /// Get all public trips.
        /// </summary>
        //[ProducesResponseType(typeof(ApiResultResponse<List<TemplateTrip>>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany,TourGuide,Traveler")]
        //[HttpGet("All")]
        //public async Task<IActionResult> GetAllTrips()
        //{
        //    var result = await Sender.Send(new GetAllTrip());
        //    return Ok(result);
        //}

        /// <summary>
        /// Search for public trips.
        /// </summary>
        [ProducesResponseType(typeof(ApiResultResponse<List<TemplateTrip>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("Search")]
        public async Task<IActionResult> SearchForTrip([FromQuery] TripFilter dto)
        {
            var result = await Sender.Send(new GetPubTripSpecQuery(dto));
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany,TourGuide,Traveler")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePrivateTrip(int id)
        {
            var result = await Sender.Send(new DeletePrivateTrip(id,GetUserId().Value));
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

        [ProducesResponseType(typeof(ApiResultResponse<List<TemplateTrip>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("Search")]
        public async Task<IActionResult> SearchForTrip([FromQuery] TripFilter dto)
        {
            var result = await Sender.Send(new GetPrivTripSpecQuery(dto));
            return Ok(result);
        }
    }
}
