using ApplicationBusiness.Dtos.Profile;
using Domain.Abstraction;
using Domain.BaseResponce;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        //private IGoogleDriveRepo googleDriveRepo { get; set; }

        //public TestController(IGoogleDriveRepo googleDriveRepo)
        //{
        //    this.googleDriveRepo = googleDriveRepo;
        //}



        //[HttpPost]
        //public async Task<ActionResult<ApiResponse>> test([FromForm] BusinessGalary businessGalaries) 
        //{
        //    return await googleDriveRepo.UploadFile("Test", businessGalaries.Photo);
        //}
        private IPhotoService photoService;

        public TestController(IPhotoService photoService)
        {
            this.photoService = photoService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> test([FromForm] BusinessGalary businessGalaries)
        {
            return await photoService.AddPhotoAsync(businessGalaries.Photo);
        }

    }
}
