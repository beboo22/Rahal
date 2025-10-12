using ApplicationBusiness.Dtos.Profile;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public void test([FromForm] BusinessGalary BusinessGalaries) {
        }
    }
}
