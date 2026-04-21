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
using ApplicationBusiness.Fetures.Authentication.Query.Models;

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

            if (result.statusCode != StatusCodes.Status201Created)
                return Ok(result);

            // ✅ 1. نجيب refresh token من الكوكي
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Ok(result); // أو ترجع error لو حابب

            // ✅ 2. نطلع token جديد
            var refreshResult = await Sender.Send(new RefreshTokenModel(refreshToken));

            if (refreshResult is not JwtAuthResponse jwtResponse || jwtResponse.statusCode != 200)
                return Ok(result); // fallback

            // ✅ 3. نحدث الكوكي
            Response.Cookies.Append("refreshToken", jwtResponse.Token.RefreshToken, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            // ✅ 4. نرجع access token الجديد
            return Ok(new ApiResultResponse<object>(
                200,
                new
                {
                    Profile = result is ApiResultResponse<TemplateTravelComapny> typereuslt ? typereuslt.Data : null,
                    AccessToken = jwtResponse.Token.AccessToken,
                    refreshToken = jwtResponse.Token.RefreshToken
                },
                "Profile created & token refreshed"
            ));


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

            if (result.statusCode != StatusCodes.Status201Created)
                return Ok(result);

            // ✅ 1. نجيب refresh token من الكوكي
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Ok(result); // أو ترجع error لو حابب

            // ✅ 2. نطلع token جديد
            var refreshResult = await Sender.Send(new RefreshTokenModel(refreshToken));

            if (refreshResult is not JwtAuthResponse jwtResponse || jwtResponse.statusCode != 200)
                return Ok(result); // fallback

            // ✅ 3. نحدث الكوكي
            Response.Cookies.Append("refreshToken", jwtResponse.Token.RefreshToken, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            // ✅ 4. نرجع access token الجديد
            return Ok(new ApiResultResponse<object>(
                200,
                new
                {
                    Profile = result is ApiResultResponse<TemplateTourGuide> typereuslt ? typereuslt.Data : null,
                    AccessToken = jwtResponse.Token.AccessToken,
                    refreshToken = jwtResponse.Token.RefreshToken
                },
                "Profile created & token refreshed"
            ));
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

            if (result.statusCode != StatusCodes.Status201Created)
                return Ok(result);

            // ✅ 1. نجيب refresh token من الكوكي
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Ok(result); // أو ترجع error لو حابب

            // ✅ 2. نطلع token جديد
            var refreshResult = await Sender.Send(new RefreshTokenModel(refreshToken));

            if (refreshResult is not JwtAuthResponse jwtResponse || jwtResponse.statusCode != 200)
                return Ok(result); // fallback

            // ✅ 3. نحدث الكوكي
            Response.Cookies.Append("refreshToken", jwtResponse.Token.RefreshToken, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            // ✅ 4. نرجع access token الجديد
            return Ok(new ApiResultResponse<object>(
                200,
                new
                {
                    Profile = result is ApiResultResponse<TemplateTraveler> typereuslt ? typereuslt.Data:null,
                    AccessToken = jwtResponse.Token.AccessToken,
                    refreshToken = jwtResponse.Token.RefreshToken
                },
                "Profile created & token refreshed"
            ));
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
