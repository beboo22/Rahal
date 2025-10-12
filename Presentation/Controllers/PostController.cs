using ApplicationBusiness.Dtos.Profile;
using ApplicationBusiness.Fetures.Profile.Command.Models;
using ApplicationBusiness.Fetures.Profile.Command;
using Domain.BaseResponce;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ApplicationBusiness.Fetures.PostService.Command.Models;
using ApplicationBusiness.Dtos.Post;
using ApplicationBusiness.Fetures.Profile.Query.Models;
using ApplicationBusiness.Fetures.PostService.Query.Models;
using ApplicationBusiness.Fetures.PostService.Query.Response;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HiringPostController : ApiController
    {
        public HiringPostController(ISender sender) : base(sender)
        {

        }

        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany")]
        public async Task<IActionResult> CreatePost([FromForm] AddHiringPostControllerDto dto)
        {
            var result = await Sender.Send(new AddHiringPostCommand(new AddHiringPostDto
            {
                PhotoUrl="",
                Requirements= dto.Requirements,
                Description=dto.Description,
                Status=dto.Status,
                Title=dto.Title,
            }, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))));
            return Ok(result);
        }
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany")]
        public async Task<IActionResult> UpdatePost([FromBody] UpdateHiringPostDto dto)
        {
            var result = await Sender.Send(new UpdateHiringPostCommand(dto, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))));
            return Ok(result);
        }
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var result = await Sender.Send(new DeleteHiringPostCommand(id, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))));
            return Ok(result);
        }


        [ProducesResponseType(typeof(ApiResultResponse<List<HiringPostTemplate>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPost([FromQuery] string title)
        {
            var result = await Sender.Send(new GetExperiencePostByTitle(title));
            return Ok(result);
        }
        [ProducesResponseType(typeof(ApiResultResponse<List<HiringPostTemplate>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet("Bydate")]
        [Authorize]
        public async Task<IActionResult> GetPost([FromQuery] DateTime date)
        {
            var result = await Sender.Send(new GetHiringPost(date));
            return Ok(result);
        }

    }
    [Route("api/[controller]")]
    [ApiController]
    public class ExperiencePostController : ApiController
    {
        public ExperiencePostController(ISender sender) : base(sender)
        {

        }

        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany")]
        public async Task<IActionResult> CreatePost([FromForm] AddExperiencePostControllerDto dto)
        {
            var result = await Sender.Send(new AddExperiencePostCommand(new AddExperiencePostDto
            {
                PhotoUrl = "",
                Budget = dto.Budget,
                Description = dto.Description,
                City = dto.City,
                Country = dto.Country,
                TipsAndRecommendations = dto.TipsAndRecommendations,
                Title = dto.Title,
            }, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))));
            return Ok(result);
        }

        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany")]
        public async Task<IActionResult> UpdatePost([FromBody] UpdateExperiencePostDto dto)
        {
            var result = await Sender.Send(new UpdateExperiencePostCommand(dto, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))));
            return Ok(result);
        }
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var result = await Sender.Send(new DeleteExperiencePostCommand(id, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))));
            return Ok(result);
        }

        [ProducesResponseType(typeof(ApiResultResponse<List<ExperiencePostTemplate>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPost([FromQuery]string title)
        {
            var result = await Sender.Send(new GetExperiencePostByTitle(title));
            return Ok(result);
        }
        [ProducesResponseType(typeof(ApiResultResponse<List<ExperiencePostTemplate>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet("Bydate")]
        [Authorize]
        public async Task<IActionResult> GetPost([FromQuery]DateTime date)
        {
            var result = await Sender.Send(new GetExperiencePost(date));
            return Ok(result);
        }

    }
}
