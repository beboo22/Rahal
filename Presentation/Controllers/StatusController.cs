using ApplicationBusiness.Dtos.Status;
using ApplicationBusiness.Fetures.StatusService.Command.Model;
using ApplicationBusiness.Fetures.StatusService.Qurey.res;
using Domain.BaseResponce;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Assuming authentication is required since you retrieve User ID
    public class StatusController : ControllerBase
    {
        private readonly ISender _sender;

        public StatusController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates a new status with an uploaded file.
        /// </summary>
        /// <param name="dto">The status details and file to upload.</param>
        /// <returns>The created status details.</returns>
        /// <response code="200">Returns the newly created status.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="500">If an internal server error occurs during upload or database commit.</response>
        [HttpPost("add")]
        [ProducesResponseType(typeof(ApiResultResponse<TemplateStatus>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<IActionResult> AddStatus([FromForm] AddStatusDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _sender.Send(new AddStatus(dto, userId.Value));

            return result.statusCode == 200 ? Ok(result) : StatusCode(result.statusCode, result);
        }

        /// <summary>
        /// Deletes a specific status.
        /// </summary>
        /// <param name="id">The ID of the status to delete.</param>
        /// <returns>A status code indicating success or failure.</returns>
        /// <response code="200">Status deleted successfully.</response>
        /// <response code="403">If the user is not the creator of the status.</response>
        /// <response code="404">If the status ID is not found.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 403)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _sender.Send(new DeleteStatus(id, userId.Value));

            return result.statusCode switch
            {
                200 => Ok(result),
                403 => Forbid(),
                404 => NotFound(result),
                _ => StatusCode(result.statusCode, result)
            };
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out int id) ? id : (int?)null;
        }
    }
}
