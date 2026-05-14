using ApplicationBusiness.Dtos.Profile;
using ApplicationBusiness.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Fetures.Profile.Command.Models;
using ApplicationBusiness.Fetures.Profile.Command;
using ApplicationBusiness.Fetures.Profile.Query.Models;
using Domain.BaseResponce;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{

    // --- TRAVEL COMPANY ---
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany")]
    public class TravelCompanyProfileController : ApiController
    {
        public TravelCompanyProfileController(ISender sender) : base(sender) { }

        [HttpPost]
        public async Task<IActionResult> CreateTravelCompanyProfile([FromForm] CreateTravelerCompanyProfileDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(new ApiResponse(401, "User ID claim missing"));

            var result = await Sender.Send(new CreateTravelerCompanyProfileCommand(dto, userId.Value));

            if (result.statusCode != StatusCodes.Status201Created) return Ok(result);

            // Handle Token Refresh Logic
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken)) return Ok(result);

            var refreshResult = await Sender.Send(new RefreshTokenModel(refreshToken));
            if (refreshResult is not JwtAuthResponse jwtResponse || jwtResponse.statusCode != 200) return Ok(result);

            Response.Cookies.Append("refreshToken", jwtResponse.Token.RefreshToken, GetCookieOptions());

            return Ok(new ApiResultResponse<object>(200, new
            {
                Profile = result is ApiResultResponse<TemplateTravelComapny> tr ? tr.Data : null,
                AccessToken = jwtResponse.Token.AccessToken,
                refreshToken = jwtResponse.Token.RefreshToken
            }, "Profile created & token refreshed"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTravelCompanyProfile([FromForm] UpdateTravelerCompanyProfileDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(new ApiResponse(401));

            var result = await Sender.Send(new UpdateTravelerCompanyProfileCommand(dto, userId.Value));
            return ProcessResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTravelCompanyProfile()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(new ApiResponse(401));

            var result = await Sender.Send(new GetTravelerCompanyProfileQuery(userId.Value));
            return ProcessResult(result);
        }
    }
}
