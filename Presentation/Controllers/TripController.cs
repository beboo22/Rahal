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
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany,TourGuide,Traveler")]
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTrip>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreatePublicTrip([FromBody] AddPublicTripDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new AddPublicTrip(dto, userId.Value));
            return ProcessResult(result);
        }

        /// <summary>
        /// Delete a public trip.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeletePublicTrip(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            // 1. Extract and parse roles cleanly using LINQ
            var roles = User.FindAll(ClaimTypes.Role)
                .Select(c => Enum.TryParse<RoleEnum>(c.Value, out var role) ? role : (RoleEnum?)null)
                .Where(r => r.HasValue)
                .Select(r => r.Value)
                .ToList();

            var result = await Sender.Send(new DeletePublicTrip(id, userId.Value, roles));
            return ProcessResult(result);
        }

        /// <summary>
        /// Search for public trips.
        /// </summary>
        [HttpGet("Search")]
        [ProducesResponseType(typeof(ApiResultResponse<List<TemplateTrip>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchForTrip([FromQuery] TripFilter dto)
        {
            var result = await Sender.Send(new GetPubTripSpecQuery(dto));
            return ProcessResult(result);
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
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany,TourGuide,Traveler")]
        [ProducesResponseType(typeof(ApiResultResponse<PrivateTemplateTrip>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreatePrivateTrip([FromBody] AddPrivateTripDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new AddPrivateTrip(dto, userId.Value));
            return ProcessResult(result);
        }

        /// <summary>
        /// Delete a private trip.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany,TourGuide,Traveler")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeletePrivateTrip(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new DeletePrivateTrip(id, userId.Value));
            return ProcessResult(result);
        }

        /// <summary>
        /// Get private trips for the current authenticated user.
        /// </summary>
        [HttpGet("User")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany,TourGuide,Traveler")]
        [ProducesResponseType(typeof(ApiResultResponse<List<PrivateTemplateTrip>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPrivateTripsByUserId()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new GetPrivateTripforUserId(userId.Value));
            return ProcessResult(result);
        }

        /// <summary>
        /// Search for private trips based on filters.
        /// </summary>
        [HttpGet("Search")]
        [ProducesResponseType(typeof(ApiResultResponse<List<TemplateTrip>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchForTrip([FromQuery] TripFilter dto)
        {
            var result = await Sender.Send(new GetPrivTripSpecQuery(dto));
            return ProcessResult(result);
        }
    }





}
