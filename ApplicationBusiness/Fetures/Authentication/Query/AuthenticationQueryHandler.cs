using Application.Abestraction;
using Application.Abstraction.message;
using Application.Fetures.Authentication.Query.Models;
using ApplicationBusiness.Fetures.Authentication.Query.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Application.Fetures.Authentication.Query
{
    public class AuthenticationQueryHandler : IQueryHandler<LoginQuery, ApiResponse>,
                                              IQueryHandler<RefreshTokenModel, ApiResponse>
    {
        private IReadGenericRepo<User> _RUR;
        private IAuthentication _auth;

        public AuthenticationQueryHandler(IReadGenericRepo<User> rUR, IAuthentication auth)
        {
            _RUR = rUR;
            _auth = auth;
        }

        public async Task<ApiResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            if (request.loginDto == null)
            {
                return new ApiResponse((int)HttpStatusCode.BadRequest, "Login data is required.");
            }
            var user = await _RUR.GetAll()
                            .Include(u => u.Roles)
                            .ThenInclude(r => r.Role)
                            .FirstOrDefaultAsync(u => u.Email == request.loginDto.Email.Trim());

            if (user == null) // Always check this first!
            {
                return new ApiResponse((int)HttpStatusCode.Unauthorized, "Invalid username or password");
            }

            // Now check password
            var hasher = new PasswordHasher<User>();
            var check = hasher.VerifyHashedPassword(user, user.PasswordHash, request.loginDto.Password);

            // Verify result — correct way
            if (check == PasswordVerificationResult.Failed)
            {
                return new ApiResponse((int)HttpStatusCode.Unauthorized, "Invalid username or password");
            }
            try
            {

                var token = await _auth.CreateTokenAsync(user);

                return new JwtAuthResponse((int)HttpStatusCode.OK, token);
            }
            catch (Exception ex)
            {
                return new ApiResultResponse<object>((int)HttpStatusCode.InternalServerError, new
                {
                    ex.Message,
                    ex.Source,
                    ex.StackTrace,
                    InnerMessage = ex.InnerException?.Message
                });
            }
        }

        public async Task<ApiResponse> Handle(RefreshTokenModel request, CancellationToken cancellationToken)
        {
            var newToken = await _auth.RefreshTokenAsync(request.refreshtoken);
            if (!newToken.HasValue)
                return new ApiResponse((int)HttpStatusCode.Unauthorized, "Invalid or expired refresh token");

            return new JwtAuthResponse((int)HttpStatusCode.OK, newToken.Value);
        }

        //public Task<ApiResponse> Handle(LogOutQuery request, CancellationToken cancellationToken)
        //{
        //    var storedToken = await _context.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        //    if (storedToken != null)
        //    {
        //        storedToken.Revoked = true;
        //        storedToken.User.LastLogoutTime = DateTime.UtcNow; // Update last logout time
        //        await _context.SaveChangesAsync();
        //    }
        //}
    }

    //public class AuthenticationQueryHandler : IQueryHandler<RefreshTokenModel, ApiResponse>
    //{
    //    private IAuthentication _auth;

    //    public AuthenticationQueryHandler(IAuthentication auth)
    //    {
    //        _auth = auth;
    //    }

    //    public async Task<ApiResponse> Handle(RefreshTokenModel request, CancellationToken cancellationToken)
    //    {
    //        var newToken = await _auth.RefreshTokenAsync(request.refreshtoken);
    //        if (!newToken.HasValue)
    //            return new ApiResponse((int)HttpStatusCode.Unauthorized, "Invalid or expired refresh token");

    //        return new JwtAuthResponse((int)HttpStatusCode.OK, newToken.Value);
    //    }

    //}

}
