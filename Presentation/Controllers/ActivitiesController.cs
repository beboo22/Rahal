using ApplicationBusiness.Abstraction.SerpApiService.Activity;
using ApplicationBusiness.Dtos.Activity;
using Domain.BaseResponce;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ActivitiesController : ApiController
    {
        private readonly IActivityService _activityService;

        public ActivitiesController(IActivityService activityService, ISender sender)
            : base(sender) // Assuming base class handles ISender
        {
            _activityService = activityService;
        }

        /// <summary>
        /// Creates a multi-day activity plan.
        /// </summary>
        /// <param name="request">The plan details including days, cities, and needs.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Plan created successfully.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost("plan")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResultResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreatePlan(
            [FromBody] CreateActivityPlanRequest request,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse(401, "User ID claim missing"));

            var result = await _activityService.CreateActivityByDayAsync(request, userId.Value, cancellationToken);

            // Standardized Switch Response
            return result.statusCode switch
            {
                200 => Ok(result),
                400 => BadRequest(result),
                401 => Unauthorized(result),
                403 => Forbid(),
                404 => NotFound(result),
                _ => StatusCode(result.statusCode, result)
            };
        }
    }
}
