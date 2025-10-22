using Application.Fetures.Authentication.Command.Models;
using Application.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Dtos.Auth;
using ApplicationBusiness.Fetures.Authentication.Command.Models;
using ApplicationBusiness.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Fetures.TripService.Query.Response;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ApiController
    {
        public AuthenticationController(ISender sender) : base(sender)
        {
            // Constructor logic if needed
        }
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            object result = await Sender.Send(new LoginQuery(loginDto));
           
            if (result is JwtAuthResponse jwtResponse)
            {
                if (jwtResponse.statusCode == (int)HttpStatusCode.OK)
                {
                    Response.Cookies.Append("refreshToken", jwtResponse.Token.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(7)
                    });

                    return Ok(new ApiResultResponse<string>(jwtResponse.statusCode,jwtResponse.Token.AccessToken,jwtResponse.message)); // token is come here i saw it in debug mode 
                }
            }
            return Ok(result);
        }
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if(registerDto.RoleIds.Any(r=>r==1))
                return Ok("Role Admin can't be register");
            var result = await Sender.Send(new signUpCommand(registerDto));
            return Ok(result);
        }
        
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {

            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null) return BadRequest("No refresh token");
            var result = await Sender.Send(new RefreshTokenModel(refreshToken));


            if (result.statusCode == (int)HttpStatusCode.OK)
                if (result is JwtAuthResponse jwtResponse)
                    return Ok(new ApiResultResponse<string>(jwtResponse.statusCode,jwtResponse.Token.AccessToken,jwtResponse.message)); // token is come here i saw it in debug mode 
                    //return Ok(new ApiResultResponse<string>(200, jwtResponse.Token.AccessToken));
            return Ok(result);
        }
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null) return BadRequest("No refresh token");
            var result = await Sender.Send(new LogOutCommand(refreshToken));



            Response.Cookies.Delete("refreshToken");
            return Ok(new { message = "Logged out" });
        }
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("request-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] ResetRequestDto dto)
        {
            object result = await Sender.Send(new ResetRequestCommand(dto));
                    

            return Ok(result);
        }
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            object result = await Sender.Send(new ResetPasswordCommand(dto));
            return Ok(result);
        }
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("RequestToVerification")]
        //[Authorize(AuthenticationSchemes= JwtBearerDefaults.AuthenticationScheme,Roles = "Traveler,TourGuide,TravelCompany")]
        public async Task<IActionResult> RequestToVerification(VerificationDtos validationDtos)
        {
            // save in wwwroot then call handler took the url of the photo
            //var email =User.FindFirstValue(ClaimTypes.Email)?.Normalize().Split("@")[0];

            //if (string.IsNullOrEmpty(email))
            //    return Unauthorized("Invalid token - user not found.");

            var email = "teest@gmail.com";

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploadsVerification");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            string SaveFile(IFormFile file, string prefix)
            {
                var extension = Path.GetExtension(file.FileName); // keep original extension
                var fileName = $"{email}_{prefix}{extension}";
                var fullPath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Return public URL
                return $"{baseUrl}/uploadsVerification/{fileName}";
            }

            var frontUrl = SaveFile(validationDtos.FrontIdentityPhotoUrl, "front");
            var identityUrl = SaveFile(validationDtos.IdentityAndUserPhotoUrl, "withuser");
            var backUrl = SaveFile(validationDtos.BackIdentityPhotoUrl, "back");

            // Pass to handler/service
            //var result = await Sender.Send(new RequestVerificationCommand(
            //    email, frontUrl, identityUrl, backUrl
            //));

            return Ok(new
            {
                frontUrl,
                identityUrl,
                backUrl
            });
        }



    }
}
