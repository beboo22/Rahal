using ApplicationBusiness.Fetures.Follow.Command.Models;
using ApplicationBusiness.Fetures.likesSerive.Command.Models;
using Domain.BaseResponce;
using Domain.Entity.PostEntity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : ApiController
    {
        public FollowController(ISender sender) : base(sender) { }


        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("follow")]
        public async Task<IActionResult> Follow([FromForm] int person)
        {
            var result = await Sender.Send(new FollowComand(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), person));
            return Ok(result);
        }
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpDelete("unfollow")]
        public async Task<IActionResult> UnFollow([FromForm] int person)
        {
            var result = await Sender.Send(new UnfollowCommand(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), person));
            return Ok(result);
        }

    }
}
