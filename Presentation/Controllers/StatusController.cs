using ApplicationBusiness.Dtos.Status;
using ApplicationBusiness.Fetures.StatusService.Command.Model;
using ApplicationBusiness.Fetures.StatusService.Qurey.res;
using Domain.BaseResponce;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ApplicationBusiness.Fetures.Authentication.Query.Response;
using ApplicationBusiness.Fetures.Authentication.Query.Models;
using Microsoft.AspNetCore.Http;
using ApplicationBusiness.Fetures.StatusService.Qurey;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Presentation.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Traveler,TourGuide,TravelerProfileController")]

    public class StatusController : ApiController
    {
        public StatusController(ISender sender) : base(sender) { }

        /// <summary>
        /// Creates a new status with an uploaded file.
        /// </summary>
        /// <param name="dto">The status details and file to upload.</param>
        [HttpPost("add")]
        [ProducesResponseType(typeof(ApiResultResponse<TemplateStatus>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddStatus([FromForm] AddStatusDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new AddStatus(dto, userId.Value));

            return ProcessResult(result);
        }

        /// <summary>
        /// Deletes a specific status.
        /// </summary>
        /// <param name="id">The ID of the status to delete.</param>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new DeleteStatus(id, userId.Value));

            return ProcessResult(result);
        }

        /// <summary>
        /// Retrieves the status of the users that the current user is following.
        /// </summary>
        [HttpGet("GetFollowingStatus")]
        [ProducesResponseType(typeof(ApiResultResponse<List<TemplateStatusOfFollowing>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFollowingStatus()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new GetStatusForFollowing(userId.Value));

            return ProcessResult(result);
        }

        /// <summary>
        /// Retrieves the status of the users that the current user is following.
        /// </summary>
        [HttpGet("GetMyStatus")]
        [ProducesResponseType(typeof(ApiResultResponse<List<TemplateStatus>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyStatus()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new getStatusForUser(userId.Value));

            return ProcessResult(result);
        }
        /// <summary>
        /// Retrieves the status of the users that the current user is following.
        /// </summary>
        [HttpGet("view")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> ViewFollowingStatus([FromQuery] int statusId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new ViewStatus(statusId,userId.Value));

            return ProcessResult(result);
        }
    }
}
