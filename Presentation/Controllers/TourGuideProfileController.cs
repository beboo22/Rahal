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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TourGuide")]
    public class TourGuideProfileController : ApiController
    {
        public TourGuideProfileController(ISender sender) : base(sender) { }

        [HttpPost]
        public async Task<IActionResult> CreateTourGuideProfile([FromForm] CreateTourGuideProfileDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(new ApiResponse(401));

            var result = await Sender.Send(new CreateTourGuideProfileCommand(dto, userId.Value));
            if (result.statusCode != StatusCodes.Status201Created) return Ok(result);

            var refreshToken = Request.Cookies["refreshToken"];
            var refreshResult = await Sender.Send(new RefreshTokenModel(refreshToken));

            if (refreshResult is JwtAuthResponse jwtResponse && jwtResponse.statusCode == 200)
            {
                Response.Cookies.Append("refreshToken", jwtResponse.Token.RefreshToken, GetCookieOptions());
                return Ok(new ApiResultResponse<object>(200, new
                {
                    Profile = result is ApiResultResponse<TemplateTourGuide> tr ? tr.Data : null,
                    AccessToken = jwtResponse.Token.AccessToken,
                    refreshToken = jwtResponse.Token.RefreshToken
                }));
            }
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTourGuideProfile([FromForm] UpdateTourGuideProfileDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new UpdateTourGuideProfileCommand(dto, userId.Value));
            return ProcessResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTourGuideProfile()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new GetTourGuideProfileQuery(userId.Value));
            return ProcessResult(result);
        }
    }
}
