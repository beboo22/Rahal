using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[cotroller]")]
    public class ApiController : ControllerBase
    {
        public ISender Sender { get; set; }

        public ApiController(ISender sender)
        {
            Sender = sender;
        }
    }
}
