using ApplicationBusiness.Fetures.CommentService.Command.Model;
using ApplicationBusiness.Fetures.likesSerive.Command.Models;
using ApplicationBusiness.Fetures.PostService.Query.Response;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Traveler,TourGuide,TravelerProfileController")]

    public class CommentController : ApiController
    {
        public CommentController(ISender sender) : base(sender) { }

        /// <summary>
        /// Adds a comment to an Experience post.
        /// </summary>
        [ProducesResponseType(typeof(ApiResultResponse<TemplateComment>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpPost("commentToExperience")]
        public async Task<IActionResult> AddCommnet([FromForm] int postId, string msg)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new AddcommentToExperiencePost(postId,
                new ApplicationBusiness.Dtos.Post.CommnetDto
                {
                    UserId = userId.Value,
                    Msg = msg
                }));

            return ProcessResult(result);
        }

        /// <summary>
        /// Adds a comment to a Hiring post.
        /// </summary>
        [ProducesResponseType(typeof(ApiResultResponse<TemplateComment>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpPost("/commentToHiring")] // Note: Leading slash makes this an absolute path (api/commentToHiring)
        public async Task<IActionResult> AddCommnetHiring([FromForm] int postId, string msg)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new AddcommentToHiringPost(postId,
                new ApplicationBusiness.Dtos.Post.CommnetDto
                {
                    UserId = userId.Value,
                    Msg = msg
                }));

            return ProcessResult(result);
        }
    }

}
