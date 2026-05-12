using ApplicationBusiness.Abstraction.SerpApiService.Activity;
using ApplicationBusiness.Dtos.Activity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivitiesController(IActivityService activityService)
            => _activityService = activityService;

        /// <summary>
        /// POST /api/activities/plan
        /// Body example:
        /// {
        ///   "days": [
        ///     {
        ///       "dayNumber": 1,
        ///       "city": "Cairo",
        ///       "needs": ["Breakfast", "Activities", "Lunch", "Mall", "Dinner"],
        ///       "language": "en",
        ///       "countryCode": "eg"
        ///     },
        ///     {
        ///       "dayNumber": 2,
        ///       "city": "Cairo",
        ///       "needs": ["Breakfast", "Lunch", "Activities", "Dinner"],
        ///       "language": "en",
        ///       "countryCode": "eg"
        ///     },
        ///     {
        ///       "dayNumber": 3,
        ///       "city": "Alexandria",
        ///       "needs": ["Breakfast", "Activities", "Lunch", "Mall", "Dinner"],
        ///       "language": "en",
        ///       "countryCode": "eg"
        ///     }
        ///   ]
        /// }
        /// </summary>
        [HttpPost("plan")]
        [Authorize]
        public async Task<IActionResult> CreatePlan(
            [FromBody] CreateActivityPlanRequest request,
            CancellationToken cancellationToken)
        {
            var userid = GetUserId();
            if (!userid.HasValue) 
                return Unauthorized();
            var result = await _activityService.CreateActivityByDayAsync(request, userid.Value,cancellationToken);
            return result.statusCode == 200 ? Ok(result) : StatusCode(result.statusCode, result);
        }


        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out int id) ? id : (int?)null;
        }

    }
}
