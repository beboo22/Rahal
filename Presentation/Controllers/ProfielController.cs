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
using Domain.Entity.Identity;

namespace Presentation.Controllers
{
    


    // --- TRAVELER ---
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Traveler,TourGuide,TravelerProfileController")]
    public class ProfileController : ApiController
    {
        public ProfileController(ISender sender) : base(sender) { }



        /// <summary>
        /// Retrieves the profile of the current user based on their specific role.
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ApiResultResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            // 1. Get the role claim value
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            // 2. Initialize a variable to hold the result
            dynamic result = null;

            // 3. Route to the correct query based on role
            // Using string comparison or Enum parsing depending on how your claims are stored
            if (userRole == RoleEnum.Traveler.ToString())
            {
                result = await Sender.Send(new GetTravelerProfileQuery(userId.Value));
            }
            else if (userRole == RoleEnum.TravelCompany.ToString())
            {
                result = await Sender.Send(new GetTravelerCompanyProfileQuery(userId.Value));
            }
            else if (userRole == RoleEnum.TourGuide.ToString())
            {
                result = await Sender.Send(new GetTourGuideProfileQuery(userId.Value));
            }
            else
            {
                return BadRequest(new ApiResponse(400, "User role is not recognized."));
            }

            // 4. Return using your standardized switch logic
            return ProcessResult(result);
        }
    }


}
