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
        public AuthenticationController(ISender sender) : base(sender) { }

        /// <summary>
        /// Login using email.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResultResponse<DateTime>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResultResponse<double>), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await Sender.Send(new LoginQuery(dto));

            // Using switch-based processing
            return result.statusCode switch
            {
                200 => Ok(result),
                429 => StatusCode(429, result), // Custom cooldown response
                404 => NotFound(result),
                _ => ProcessResult(result)
            };
        }

        /// <summary>
        /// Register new user.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (dto.Roles.Any(r => r == (int)RoleEnum.Admin))
                return BadRequest(new ApiResponse(400, "Admin role cannot be self-assigned."));

            var result = await Sender.Send(new signUpCommand(dto));
            return ProcessResult(result);
        }

        /// <summary>
        /// Refresh access token using refresh token from cookie.
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(ApiResultResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new ApiResponse(401, "Refresh token not found."));

            var result = await Sender.Send(new RefreshTokenModel(refreshToken));

            if (result is JwtAuthResponse jwtResponse && result.statusCode == 200)
            {
                Response.Cookies.Append("refreshToken", jwtResponse.Token.RefreshToken, GetCookieOptions());

                return Ok(new ApiResultResponse<string>(
                    200,
                    jwtResponse.Token.AccessToken,
                    "Token refreshed successfully"
                ));
            }

            return ProcessResult(result);
        }

        /// <summary>
        /// Logout user and revoke refresh token.
        /// </summary>
        [HttpPost("logout")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new ApiResponse(400, "Refresh token not found."));

            var result = await Sender.Send(new LogOutCommand(refreshToken));

            Response.Cookies.Delete("refreshToken");

            return ProcessResult(result);
        }

        /// <summary>
        /// Verify OTP and generate JWT tokens.
        /// </summary>
        [HttpPost("verify-otp")]
        [ProducesResponseType(typeof(ApiResultResponse<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command)
        {
            var result = await Sender.Send(command);

            if (result is ApiResultResponse<UserDto> successResponse && result.statusCode == 200)
            {
                Response.Cookies.Append("refreshToken", successResponse.Data.Token.RefreshToken, GetCookieOptions());
                return Ok(result);
            }

            return ProcessResult(result);
        }

        // This helper ensures we use the Switch Expression pattern for all standard calls
        //private IActionResult ProcessResult(dynamic result)
        //{
        //    return result.statusCode switch
        //    {
        //        200 => Ok(result),
        //        201 => Created(string.Empty, result),
        //        400 => BadRequest(result),
        //        401 => Unauthorized(result),
        //        403 => Forbid(),
        //        404 => NotFound(result),
        //        _ => StatusCode((int)result.statusCode, result)
        //    };
        //}

        //private CookieOptions GetCookieOptions() => new CookieOptions
        //{
        //    HttpOnly = false, // Set to true in production for better security
        //    Secure = true,
        //    SameSite = SameSiteMode.Strict,
        //    Expires = DateTime.UtcNow.AddDays(7)
        //};
    }

}
