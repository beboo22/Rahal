using ApplicationBusiness.Dtos.Profile;
using ApplicationBusiness.Fetures.Profile.Command.Models;
using ApplicationBusiness.Fetures.Profile.Command;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany")]
    public class TravelCompanyProfileController : ApiController
    {
        public TravelCompanyProfileController(ISender sender) : base(sender) { }

        // POST: api/travelcompanyprofile
        [HttpPost]
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTravelComapny>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTravelCompanyProfile([FromForm] CreateTravelerCompanyProfileDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim missing"));

            var result = await Sender.Send(new CreateTravelerCompanyProfileCommand(dto, userId.Value));
            return Ok(result);
        }

        // PUT: api/travelcompanyprofile
        [HttpPut]
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTravelComapny>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTravelCompanyProfile([FromBody] UpdateTravelerCompanyProfileDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim missing"));

            var result = await Sender.Send(new UpdateTravelerCompanyProfileCommand(dto, userId.Value));
            return Ok(result);
        }

        // GET: api/travelcompanyprofile
        [HttpGet]
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTravelComapny>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTravelCompanyProfile()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim missing"));

            var result = await Sender.Send(new GetTravelerCompanyProfileQuery(userId.Value));
            return Ok(result);
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out int id) ? id : (int?)null;
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TourGuide")]
    public class TourGuideProfileController : ApiController
    {
        public TourGuideProfileController(ISender sender) : base(sender) { }

        // POST: api/tourguideprofile
        [HttpPost]
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTourGuide>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTourGuideProfile([FromForm] CreateTourGuideProfileDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim missing"));

            var result = await Sender.Send(new CreateTourGuideProfileCommand(dto, userId.Value));
            return Ok(result);
        }

        // PUT: api/tourguideprofile
        [HttpPut]
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTourGuide>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTourGuideProfile([FromBody] UpdateTourGuideProfileDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim missing"));

            var result = await Sender.Send(new UpdateTourGuideProfileCommand(dto, userId.Value));
            return Ok(result);
        }

        // GET: api/tourguideprofile
        [HttpGet]
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTourGuide>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTourGuideProfile()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim missing"));

            var result = await Sender.Send(new GetTourGuideProfileQuery(userId.Value));
            return Ok(result);
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out int id) ? id : (int?)null;
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Traveler")]
    public class TravelerProfileController : ApiController
    {
        public TravelerProfileController(ISender sender) : base(sender) { }

        // POST: api/travelerprofile
        [HttpPost]
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTraveler>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTravelerProfile([FromBody] CreateTravelerProfileDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim missing"));

            var result = await Sender.Send(new CreateTravelerProfileCommand(dto, userId.Value));
            return Ok(result);
        }

        // PUT: api/travelerprofile
        [HttpPut]
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTraveler>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTravelerProfile([FromBody] UpdateTravelerProfileDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim missing"));

            var result = await Sender.Send(new UpdateTravelerProfileCommand(dto, userId.Value));
            return Ok(result);
        }

        // GET: api/travelerprofile
        [HttpGet]
        [ProducesResponseType(typeof(ApiResultResponse<TemplateTraveler>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTravelerProfile()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized, "User ID claim missing"));

            var result = await Sender.Send(new GetTravelerProfileQuery(userId.Value));
            return Ok(result);
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out int id) ? id : (int?)null;
        }
    }
}
