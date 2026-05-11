using ApplicationBusiness.Dtos.Post;
using ApplicationBusiness.Fetures.likesSerive.Command.Models;
using ApplicationBusiness.Fetures.likesSerive.Query;
using ApplicationBusiness.Fetures.likesSerive.Query.Models;
using ApplicationBusiness.Fetures.PostService.Command.Models;
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
    [Authorize]
    public class LikesController : ApiController
    {
        public LikesController(ISender sender) : base(sender) { }
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("Likes")]
        public async Task<IActionResult> AddLike([FromForm] int postId, LikeType likeType)
        {
            var result = await Sender.Send(new AddLike(postId, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),likeType));
            return Ok(result);
        }
        
        
        [ProducesResponseType(typeof(ApiResultResponse<List<TemplateuserLikePost>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet("WhoLikes")]
        public async Task<IActionResult> GetLike([FromQuery] int postId)
        {
            var result = await Sender.Send(new GetUserLikeToPost(postId));
            return Ok(result);
        }






    }
}
