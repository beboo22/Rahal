using ApplicationBusiness.Fetures.Follow.Command.Models;
using ApplicationBusiness.Fetures.likesSerive.Command.Models;
using Domain.BaseResponce;
using Domain.Entity.PostEntity;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Traveler,TourGuide,TravelerProfileController")]

    public class FollowController : ApiController
    {
        public FollowController(ISender sender) : base(sender) { }

        /// <summary>
        /// Follows a specific user.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("follow")]
        public async Task<IActionResult> Follow([FromForm] int person)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new FollowComand(userId.Value, person));

            return ProcessResult(result);
        }

        /// <summary>
        /// Unfollows a specific user.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpDelete("unfollow")]
        public async Task<IActionResult> UnFollow([FromForm] int person)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new UnfollowCommand(userId.Value, person));

            return ProcessResult(result);
        }
    }
}
