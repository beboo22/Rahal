using Application.Fetures.Authentication.Command.Models;
using Application.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Dtos.Auth;
using ApplicationBusiness.Fetures.Authentication.Command.Models;
using ApplicationBusiness.Fetures.Authentication.Query.Models;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    [Tags("Authentication")]
    public class AuthenticationController : ApiController
    {
        public AuthenticationController(ISender sender) : base(sender)
        {
        }

        /// <summary>
        /// Login using email.
        /// </summary>
        /// <remarks>
        /// Flow:
        /// - If user not found → 404
        /// - If OTP cooldown active → 429 (returns remaining minutes)
        /// - If success → 200 (returns next allowed OTP time)
        /// </remarks>
        [HttpPost("login")]

        [ProducesResponseType(typeof(ApiResultResponse<DateTime>), StatusCodes.Status200OK)]

        [ProducesResponseType(typeof(ApiResultResponse<double>), StatusCodes.Status429TooManyRequests)]

        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]

        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]

        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await Sender.Send(new LoginQuery(dto));

                return StatusCode(result.statusCode, result);
            //if (result is not JwtAuthResponse jwtResponse)
            //    return StatusCode(result.statusCode, result);

            //if (jwtResponse.statusCode != (int)HttpStatusCode.OK)
            //    return StatusCode(jwtResponse.statusCode, jwtResponse);

            //// Set Refresh Token Cookie
            //Response.Cookies.Append("refreshToken", jwtResponse.Token.RefreshToken, new CookieOptions
            //{
            //    HttpOnly = true,
            //    Secure = true,
            //    SameSite = SameSiteMode.Strict,
            //    Expires = DateTime.UtcNow.AddDays(7)
            //});

            //return Ok(new ApiResultResponse<string>(
            //    result.statusCode,
            //    result.Token.AccessToken,
            //    "Login successful"
            //));
        }

        /// <summary>
        /// Register new user.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (dto.Roles.Any(r => r == (int)RoleEnum.Admin))
                return BadRequest(new ApiResponse(400, "Admin role cannot be self-assigned."));

            var result = await Sender.Send(new signUpCommand(dto));

            return StatusCode(result.statusCode, result);
        }

        /// <summary>
        /// Refresh access token using refresh token from cookie.
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(ApiResultResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new ApiResponse(401, "Refresh token not found."));

            var result = await Sender.Send(new RefreshTokenModel(refreshToken));

            if (result is not JwtAuthResponse jwtResponse)
                return StatusCode(result.statusCode, result);

            if (jwtResponse.statusCode != (int)HttpStatusCode.OK)
                return StatusCode(jwtResponse.statusCode, jwtResponse);
            if (result.statusCode == 200)
            {

                Response.Cookies.Append("refreshToken", jwtResponse.Token.RefreshToken, new CookieOptions
                {

                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });
            }
            return Ok(new ApiResultResponse<string>(
                jwtResponse.statusCode,
                jwtResponse.Token.AccessToken,
                "Token refreshed successfully"
            ));
        }

        /// <summary>
        /// Logout user and revoke refresh token.
        /// </summary>
        [HttpPost("logout")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new ApiResponse(400, "Refresh token not found."));

            var result = await Sender.Send(new LogOutCommand(refreshToken));

            Response.Cookies.Delete("refreshToken");

            return StatusCode(result.statusCode, result);
        }

        /// <summary>
        /// Verify OTP and generate JWT tokens.
        /// </summary>
        [HttpPost("verify-otp")]
        [ProducesResponseType(typeof(ApiResultResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command)
        {
            var result = await Sender.Send(command);


            if (result is not ApiResultResponse<UserDto> jwtResponse)
                return StatusCode(result.statusCode, result);


            if (result.statusCode == 200)
            {

                Response.Cookies.Append("refreshToken", jwtResponse.Data.Token.RefreshToken, new CookieOptions
                {
                    
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });
            }
            return Ok(result);
        }
    }

    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    //    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    //    [HttpPost("RequestToVerification")]
    //    //[Authorize(AuthenticationSchemes= JwtBearerDefaults.AuthenticationScheme,Roles = "Traveler,TourGuide,TravelCompany")]
    //    public async Task<IActionResult> RequestToVerification(VerificationDtos validationDtos)
    //    {
    //        // save in wwwroot then call handler took the url of the photo
    //        //var email =User.FindFirstValue(ClaimTypes.Email)?.Normalize().Split("@")[0];

    //        //if (string.IsNullOrEmpty(email))
    //        //    return Unauthorized("Invalid token - user not found.");

    //        var email = "teest@gmail.com";

    //        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploadsVerification");
    //        if (!Directory.Exists(uploadPath))
    //            Directory.CreateDirectory(uploadPath);
    //        var baseUrl = $"{Request.Scheme}://{Request.Host}";

    //        string SaveFile(IFormFile file, string prefix)
    //        {
    //            var extension = Path.GetExtension(file.FileName); // keep original extension
    //            var fileName = $"{email}_{prefix}{extension}";
    //            var fullPath = Path.Combine(uploadPath, fileName);

    //            using (var stream = new FileStream(fullPath, FileMode.Create))
    //            {
    //                file.CopyTo(stream);
    //            }

    //            // Return public URL
    //            return $"{baseUrl}/uploadsVerification/{fileName}";
    //        }

    //        var frontUrl = SaveFile(validationDtos.FrontIdentityPhotoUrl, "front");
    //        var identityUrl = SaveFile(validationDtos.IdentityAndUserPhotoUrl, "withuser");
    //        var backUrl = SaveFile(validationDtos.BackIdentityPhotoUrl, "back");

    //        // Pass to handler/service
    //        //var result = await Sender.Send(new RequestVerificationCommand(
    //        //    email, frontUrl, identityUrl, backUrl
    //        //));

    //        return Ok(new
    //        {
    //            frontUrl,
    //            identityUrl,
    //            backUrl
    //        });
    //    }


    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    //    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    //    [HttpPost("request-reset")]
    //    public async Task<IActionResult> RequestPasswordReset([FromBody] ResetRequestDto dto)
    //    {
    //        object result = await Sender.Send(new ResetRequestCommand(dto));


    //        return Ok(result);
    //    }
    //    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    //    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    //    [HttpPost("reset-password")]
    //    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    //    {
    //        object result = await Sender.Send(new ResetPasswordCommand(dto));
    //        return Ok(result);
    //    }


}
