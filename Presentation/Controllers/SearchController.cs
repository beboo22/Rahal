using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    using Application.Fetures.Authentication.Query.Models;
    using ApplicationBusiness.Fetures.Authentication.Query;
    using ApplicationBusiness.Fetures.Search.Query;
    using ApplicationBusiness.Fetures.StatusService.Command.Model;
    using ApplicationBusiness.RealTimeservice.NotificationService;
    using ApplicationBusiness.service;
    using Domain.BaseResponce;
    using Domain.Entity.Identity;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    // ✅ صلحنا الاسم هنا عشان يبقى طبيعي ومتوافق مع الـ Routing
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Traveler,TourGuide,TravelerProfileController")]

    public class SearchController : ApiController
    {
        public SearchController(ISender sender) : base(sender)
        {
        }

        /// <summary>
        /// get hotel and flight fir spec country
        /// </summary>
        [HttpGet("Hotel_Flight")]
        [ProducesResponseType(typeof(ApiResultResponse<HotelFlightResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        // ✅ باصي الـ record كلو حتة واحدة جوه FromQuery والـ .NET هيفكّه لوحده في الـ URL
        public async Task<IActionResult> GetHotel_flight([FromQuery] GetHotelflight request)
        {
            var result = await Sender.Send(request);
            return ProcessResult(result);
        }
    }
}
