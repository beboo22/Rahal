using MediatR;
using Microsoft.AspNetCore.Mvc;
using Domain.BaseResponce;
using ApplicationBusiness.Fetures.BusinessGalary.Command;
using Microsoft.AspNetCore.Authorization;
using ApplicationBusiness.Fetures.BusinessGalary.Query;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Presentation.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TourGuide")]

    public class TourguideBusinessGalaryController : ApiController
    {
        public TourguideBusinessGalaryController(ISender sender) : base(sender)
        {
        }

        #region Tour Guide

        [HttpDelete("tour-guide")]
        public async Task<IActionResult> DeleteTourGuideBusinessGalary(
    int businessGalaryId)
        {
            var result = await Sender.Send(
                new DeleteTourGuideBusinessGalary(businessGalaryId));

            return ProcessResult(result);
        }


        [HttpPost("tour-guide")]
        public async Task<IActionResult> CreateTourGuideBusinessGalary(
            [FromForm] BusinessGalary dto)
        {
            int? tourGuideId = GetUserId();
            if (tourGuideId == null) return Unauthorized(new ApiResponse(401));

            var result = await Sender.Send(
                new createListTourGuideBusinessGalary(
                    tourGuideId.Value,
                    dto));

            return ProcessResult(result);
        }

        [HttpPut("tour-guide")]
        public async Task<IActionResult> UpdateTourGuideBusinessGalary(
            int businessGalaryId,
            [FromForm] BusinessGalary dto)
        {
            var result = await Sender.Send(
                new UpdateTourGuideBusinessGalary(
                    businessGalaryId,
                    dto));

            return ProcessResult(result);
        }

        #endregion




        [HttpGet]
        public async Task<IActionResult> GetTourguideBusinessGalaryEndpoint()
        {
            var tourGuideId = GetUserId();
            if (tourGuideId == null)
                return Unauthorized(new ApiResponse(401));
            var result = await Sender.Send(
                new GetTourguideBusinessGalary(tourGuideId.Value));
            return ProcessResult(result);
        }


    }
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "TravelerProfileController")]

    public class TravelCompanyBusinessGalaryController : ApiController
    {
        public TravelCompanyBusinessGalaryController(ISender sender) : base(sender)
        {
        }



        #region Travel Company

        [HttpDelete("travel-company")]
        public async Task<IActionResult> DeleteTravelCompanyBusinessGalary(
    int businessGalaryId)
        {
            var result = await Sender.Send(
                new DeleteTravelCompanyBusinessGalary(businessGalaryId));

            return ProcessResult(result);
        }


        [HttpPost("travel-company")]
        public async Task<IActionResult> CreateTravelCompanyBusinessGalary(
            [FromForm] BusinessGalary dto)
        {
            int? travelCompanyId = GetUserId();
            if (travelCompanyId == null) return Unauthorized(new ApiResponse(401));
            var result = await Sender.Send(
                new CreateListTravelCompanyBusinessGalary(
                    travelCompanyId.Value,
                    dto));

            return ProcessResult(result);
        }

        [HttpPut("travel-company")]
        public async Task<IActionResult> UpdateTravelCompanyBusinessGalary(
            int businessGalaryId,
            [FromForm] BusinessGalary dto)
        {
            var result = await Sender.Send(
                new UpdateTravelCompanyBusinessGalary(
                    businessGalaryId,
                    dto));

            return ProcessResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTcBusinessGalaryEndpoint(int businessGalaryId)
        {
            var tourGuideId = GetUserId();
            if (tourGuideId == null)
                return Unauthorized(new ApiResponse(401));
            var result = await Sender.Send(
                new GetTravelCompanyBusinessGalary(tourGuideId.Value));
            return ProcessResult(result);
        }

        #endregion
    }


}
