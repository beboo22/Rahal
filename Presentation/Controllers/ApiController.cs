using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        protected int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out int id) ? id : (int?)null;
        }

        protected CookieOptions GetCookieOptions() => new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        protected IActionResult ProcessResult(dynamic result)
        {
            return result.statusCode switch
            {
                200 => Ok(result),
                201 => Created(string.Empty, result),
                403 => StatusCode(403, result),
                //403 => Forbid(string.Empty,result),
                404 => NotFound(result),
                401 => Unauthorized(result),
                _ => StatusCode((int)result.statusCode, result)
            };
        }

    }
}
