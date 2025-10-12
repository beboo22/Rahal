using ApplicationBusiness.Dtos.Profile;
using ApplicationBusiness.Fetures.Profile.Command;
using ApplicationBusiness.Fetures.Profile.Command.Models;
using ApplicationBusiness.Fetures.Profile.Query.Models;
using Domain.BaseResponce;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfielController : ApiController
    {
        public ProfielController(ISender sender) : base(sender)
        {
            // Constructor logic if needed
        }
        // Add profile-related actions here
        //post /trvelercompanyprofile
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTravelComapny>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("travelcompanyprofile")]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme,Roles = "TravelCompany")]
        //[Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateTravelerCompanyProfile([FromForm] CreateTravelerCompanyProfileDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim is missing from token"));

            if (!int.TryParse(userIdClaim.Value, out int id))
                return BadRequest(new ApiResponse((int)HttpStatusCode.Unauthorized, "Invalid User ID format in token"));
            var result = await Sender.Send(new CreateTravelerCompanyProfileCommand(dto, id));
            return Ok(result);
        }
        //post /trvelerprofile
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTraveler>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme,Roles = "Traveler")]
        [HttpPost("travelerprofile")]
        public async Task<IActionResult> CreateTravelerProfile([FromBody] CreateTravelerProfileDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim is missing from token"));

            if (!int.TryParse(userIdClaim.Value, out int id))
                return BadRequest(new ApiResponse((int)HttpStatusCode.Unauthorized, "Invalid User ID format in token"));
            var result = await Sender.Send(new CreateTravelerProfileCommand(dto, id));
            return Ok(result);
        }
        //tourguideprofile
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTourGuide>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]

        [HttpPost("tourguideprofile")]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme,Roles = "TourGuide")]
        public async Task<IActionResult> CreateTourGuideProfile([FromForm] CreateTourGuideProfileDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim is missing from token"));

            if (!int.TryParse(userIdClaim.Value, out int id))
                return BadRequest(new ApiResponse((int)HttpStatusCode.Unauthorized, "Invalid User ID format in token"));
            var result = await Sender.Send(new CreateTourGuideProfileCommand(dto,id));
            return Ok(result);
        }
        //put /trvelercompanyprofile
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTravelComapny>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPut("travelcompanyprofile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany")]
        public async Task<IActionResult> UpdateTravelerCompanyProfile([FromBody] UpdateTravelerCompanyProfileDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim is missing from token"));

            if (!int.TryParse(userIdClaim.Value, out int id))
                return BadRequest(new ApiResponse((int)HttpStatusCode.Unauthorized, "Invalid User ID format in token"));
            var result = await Sender.Send(new UpdateTravelerCompanyProfileCommand(dto, id));
            return Ok(result);
        }
        //put /trvelerprofile
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTraveler>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPut("travelerprofile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Traveler")]
        public async Task<IActionResult> UpdateTravelerProfile([FromBody] UpdateTravelerProfileDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim is missing from token"));

            if (!int.TryParse(userIdClaim.Value, out int id))
                return BadRequest(new ApiResponse((int)HttpStatusCode.Unauthorized, "Invalid User ID format in token"));
            var result = await Sender.Send(new UpdateTravelerProfileCommand(dto, id));
            return Ok(result);
        }
        //put /tourguideprofile
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTourGuide>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPut("tourguideprofile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TourGuide")]
        public async Task<IActionResult> UpdateTourGuideProfile([FromBody] UpdateTourGuideProfileDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim is missing from token"));

            if (!int.TryParse(userIdClaim.Value, out int id))
                return BadRequest(new ApiResponse((int)HttpStatusCode.Unauthorized, "Invalid User ID format in token"));
            var result = await Sender.Send(new UpdateTourGuideProfileCommand(dto, id));
            return Ok(result);
        }

        //get /trvelercompanyprofile/{userId}
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTravelComapny>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet("travelcompanyprofile")]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme,Roles = "TravelCompany")]
        public async Task<IActionResult> GetTravelerCompanyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim is missing from token"));

            if (!int.TryParse(userIdClaim.Value, out int id))
                return BadRequest(new ApiResponse((int)HttpStatusCode.Unauthorized, "Invalid User ID format in token"));
            var result = await Sender.Send(new GetTravelerCompanyProfileQuery(id));
            return Ok(result);
        }
        //get /trvelerprofile/{userId}
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTraveler>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet("travelerprofile")]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme,Roles = "Traveler")]
        public async Task<IActionResult> GetTravelerProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized,"User ID claim is missing from token"));

            if (!int.TryParse(userIdClaim.Value, out int id))
                return BadRequest(new ApiResponse((int)HttpStatusCode.Unauthorized, "Invalid User ID format in token"));

            var result = await Sender.Send(new GetTravelerProfileQuery(id));

            return Ok(result);
        }

        //get /tourguideprofile/{userId}
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTourGuide>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet("tourguideprofile")]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme,Roles = "TourGuide")]
        public async Task<IActionResult> GetTourGuideProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim is missing from token"));

            if (!int.TryParse(userIdClaim.Value, out int id))
                return BadRequest(new ApiResponse((int)HttpStatusCode.Unauthorized, "Invalid User ID format in token"));
            var result = await Sender.Send(new GetTourGuideProfileQuery(id));
            return Ok(result);
        }


    }
}
