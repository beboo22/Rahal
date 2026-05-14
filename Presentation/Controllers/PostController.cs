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
        public HiringPostController(ISender sender) : base(sender) { }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreatePost([FromForm] AddHiringPostControllerDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new AddHiringPostCommand(new AddHiringPostDto
            {
                PhotoUrl = "",
                Requirements = dto.Requirements,
                Description = dto.Description,
                Status = dto.Status,
                Title = dto.Title,
            }, userId.Value));

            return ProcessResult(result);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePost([FromBody] UpdateHiringPostDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new UpdateHiringPostCommand(dto, userId.Value));
            return ProcessResult(result);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelCompany")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new DeleteHiringPostCommand(id, userId.Value));
            return ProcessResult(result);
        }

        [HttpGet("By")]
        [ProducesResponseType(typeof(ApiResultResponse<List<HiringPostTemplate>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPost([FromQuery] DateTime? Date, int? Id, string? Title, int? page, bool OrderDesBytimeCreated = false, int capacity = 5)
        {
            var result = await Sender.Send(new GetHiringSpacificationPost(Date, Id, Title, page, OrderDesBytimeCreated, capacity));
            return ProcessResult(result);
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ExperiencePostController : ApiController
    {
        public ExperiencePostController(ISender sender) : base(sender) { }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreatePost([FromForm] AddExperiencePostControllerDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new AddExperiencePostCommand(dto, userId.Value));
            return ProcessResult(result);
        }

        [HttpPut]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePost([FromBody] UpdateExperiencePostDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new UpdateExperiencePostCommand(dto, userId.Value));
            return ProcessResult(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await Sender.Send(new DeleteExperiencePostCommand(id, userId.Value));
            return ProcessResult(result);
        }

        [HttpGet("By")]
        [ProducesResponseType(typeof(ApiResultResponse<List<ExperiencePostTemplate>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPost([FromQuery] DateTime? date,
            int? id,
            string? title,
            string? country,
            string? city,
            decimal? budget, int? pageIndex, int pageSize = 5,
            bool OrderDesBytimeCreated = false)
        {
            var result = await Sender.Send(new GetExperienceSpacificationPost(date, id, title, country, city, OrderDesBytimeCreated, budget, pageIndex, pageSize));
            return ProcessResult(result);
        }
    }


}
