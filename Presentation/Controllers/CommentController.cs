using ApplicationBusiness.Fetures.CommentService.Command.Model;
using ApplicationBusiness.Fetures.likesSerive.Command.Models;
using ApplicationBusiness.Fetures.PostService.Query.Response;
using Domain.BaseResponce;
using Domain.Entity.PostEntity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ApiController
    {
        public CommentController(ISender sender) : base(sender) { }

        [ProducesResponseType(typeof(ApiResultResponse<TemplateComment>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("commentToExperience")]
        public async Task<IActionResult> AddCommnet([FromForm] int postId,string msg)
        {
            var result = await Sender.Send(new AddcommentToExperiencePost(postId, 
                new ApplicationBusiness.Dtos.Post.CommnetDto
                {
                 UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                 Msg = msg
                }));
            return Ok(result);
        }

        [ProducesResponseType(typeof(ApiResultResponse<TemplateComment>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("/commentToHiring")]
        public async Task<IActionResult> AddCommnetHiring([FromForm] int postId, string msg)
        {
            var result = await Sender.Send(new AddcommentToHiringPost(postId,
                new ApplicationBusiness.Dtos.Post.CommnetDto
                {
                    UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    Msg = msg
                }));
            return Ok(result);
        }



    }
}
