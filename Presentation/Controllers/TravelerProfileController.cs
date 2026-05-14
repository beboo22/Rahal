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
    // --- TRAVELER ---
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Traveler")]
    public class TravelerProfileController : ApiController
    {
        public TravelerProfileController(ISender sender) : base(sender) { }

        [HttpPost]
        public async Task<IActionResult> CreateTravelerProfile([FromForm] CreateTravelerProfileDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new CreateTravelerProfileCommand(dto, userId.Value));
            if (result.statusCode != StatusCodes.Status201Created) return Ok(result);

            var refreshToken = Request.Cookies["refreshToken"];
            var refreshResult = await Sender.Send(new RefreshTokenModel(refreshToken));

            if (refreshResult is JwtAuthResponse jwtResponse && jwtResponse.statusCode == 200)
            {
                Response.Cookies.Append("refreshToken", jwtResponse.Token.RefreshToken, GetCookieOptions());
                //return Ok(result);
            }
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTravelerProfile([FromForm] UpdateTravelerProfileDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new UpdateTravelerProfileCommand(dto, userId.Value));
            return ProcessResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTravelerProfile()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new GetTravelerProfileQuery(userId.Value));
            return ProcessResult(result);
        }
    }
}
