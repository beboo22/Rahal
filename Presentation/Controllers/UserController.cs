using ApplicationBusiness.Fetures.likesSerive.Query.Models;
using ApplicationBusiness.Fetures.likesSerive.Query;
using Domain.BaseResponce;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Application.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Fetures.Authentication.Query;
using ApplicationBusiness.Fetures.Authentication.Query.Response;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Enabled Authorize as these methods rely on User context
    public class UserController : ApiController
    {
        public UserController(ISender sender) : base(sender) { }

        /// <summary>
        /// Retrieves the status/likes for the current authenticated user.
        /// </summary>
        [ProducesResponseType(typeof(ApiResultResponse<List<TemplateStatusOfFollowing>>), StatusCodes.Status200OK)]
        [HttpGet("Getstatus")]
        public async Task<IActionResult> Getstatus()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse(401));

            // Note: Ensure the Command matches the intended DTO (GetUserLikeToPost vs FollowingStatus)
            var result = await Sender.Send(new GetUserLikeToPost(userId.Value));

            return ProcessResult(result);
        }

        /// <summary>
        /// Retrieves a generic profile for a specific user by their ID.
        /// </summary>
        /// <param name="UserId">The ID of the user to retrieve.</param>
        [ProducesResponseType(typeof(ApiResultResponse<TemplateGenericProfile>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetProfileById(int UserId)
        {
            var result = await Sender.Send(new GetUserById(UserId));

            return ProcessResult(result);
        }
    }
}
