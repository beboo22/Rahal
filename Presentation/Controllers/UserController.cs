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
    //[Authorize]
    public class UserController : ApiController
    {
        public UserController(ISender sender) : base(sender) { }

        [ProducesResponseType(typeof(ApiResultResponse<List<TemplateStatusOfFollowing>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet("Getstatus")]
        public async Task<IActionResult> Getstatus()
        {
            var userId= GetUserId();
            if (!userId.HasValue) 
                return Unauthorized();
            var result = await Sender.Send(new GetUserLikeToPost(userId.Value));
            return Ok(result);
        }

        [ProducesResponseType(typeof(ApiResultResponse<List<TemplateGenericProfile>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetProfileById(int UserId)
        {
            var result = await Sender.Send(new GetUserById(UserId));
            return Ok(result);
        }



        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out int id) ? id : (int?)null;
        }

    }
}
