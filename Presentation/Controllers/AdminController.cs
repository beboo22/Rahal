using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    using Application.Fetures.Authentication.Query.Models;
    using ApplicationBusiness.Fetures.AdminDashbourd.Query;
    using ApplicationBusiness.Fetures.Authentication.Command;
    using ApplicationBusiness.Fetures.Authentication.Query;
    using ApplicationBusiness.Fetures.PostService.Command;
    using ApplicationBusiness.Fetures.PostService.Query;
    using ApplicationBusiness.Fetures.Profile.Query;
    using ApplicationBusiness.RealTimeservice.NotificationService;
    using ApplicationBusiness.service;
    using Domain.BaseResponce;
    using Domain.Entity.Identity;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles ="Admin")]
    public class AdminController : ApiController
    {

        public AdminController(ISender sender) : base(sender)
        {
        }
        



        [HttpGet("GetDashbourdDataQuery")]
        public async Task<IActionResult> GetDashbourdDataQueryEndpoint()
        {

            var result = await Sender.Send(new DashbourdDataQuery());

            return ProcessResult(result);
        }


        [HttpGet("GetUnverfiredTourGuide")]
        public async Task<IActionResult> GetUnverfiredTourGuideEndpoint()
        {

            var result = await Sender.Send(new GetUnverfiredTourGuide());

            return ProcessResult(result);
        }

        [HttpGet("GetUnverfiredTraveleCompany")]
        public async Task<IActionResult> GetUnverfiredTraveleCompanyEndpoint()
        {

            var result = await Sender.Send(new GetUnverfiredTraveleCompany());

            return ProcessResult(result);
        }

        [HttpPost("verfiedTourguideAndTravelCompany")]
        public async Task<IActionResult> verfiedTourguideAndTravelCompanyEndpoint([FromQuery]int ProfileId)
        {

            var result = await Sender.Send(new verfiedTourguideAndTravelCompany(ProfileId));

            return ProcessResult(result);
        }

        
        [HttpGet("GetUnValidPost")]
        public async Task<IActionResult> GetUnValidPostEndpoint()
        {

            var result = await Sender.Send(new GetUnValidPost());

            return ProcessResult(result);
        }
        
        [HttpPut("ValidatePostContentCommand")]
        public async Task<IActionResult> ValidatePostContentCommandEndpoint([FromBody] int postId)
        {

            var result = await Sender.Send(new ValidatePostContentCommand(postId));

            return ProcessResult(result);
        }
        [HttpDelete("BlockPost")]
        public async Task<IActionResult> BlockPostEndpoint([FromBody] int postId)
        {

            var result = await Sender.Send(new BlockPost(postId));

            return ProcessResult(result);
        }

        [HttpPut("BlockUser")]
        public async Task<IActionResult> BlockUserEndpoint([FromBody] int UserId, DateTime BlockedStartDate, DateTime BlockedEndDate)
        {
            if(BlockedStartDate >= BlockedEndDate)
            {
                return BadRequest("BlockedStartDate must be earlier than BlockedEndDate.");
            }

            var result = await Sender.Send(new BlockUser(UserId,BlockedStartDate,BlockedEndDate));

            return ProcessResult(result);
        }






    }

}
