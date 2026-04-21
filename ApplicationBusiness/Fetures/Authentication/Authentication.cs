using Domain.Abstraction;
using Domain.Entity.Identity;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Application.Abestraction
{
    public class Authentication : IAuthentication
    {
        private readonly IConfiguration _config;
        private IWriteUnitOfWork _Uow;
        private IWriteGenericRepo<RefreshToken> _WRepo;
        private IReadGenericRepo<RefreshToken> _RRepo;
        private IReadGenericRepo<User> _RUR;




        public Authentication(IConfiguration config, IWriteUnitOfWork uow, IWriteGenericRepo<RefreshToken> wRepo, IReadGenericRepo<RefreshToken> rRepo, IReadGenericRepo<User> rUR)
        {
            _config = config;
            _Uow = uow;
            _WRepo = wRepo;
            _RRepo = rRepo;
            _RUR = rUR;
        }

        // Generate Access + Refresh token
        // Generate Access + Refresh token
        public async Task<(string AccessToken, string RefreshToken, DateTime Expiration)> CreateTokenAsync(User user)
        {
            // --- 1. Claims ---
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FName ?? string.Empty} {user.LName ?? string.Empty}"),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                // add claim save Isverified status
                new Claim("IsVerified", user.Isverified.ToString()),
                new Claim("IsBlocked", user.IsBlocked.ToString())
            };
            //var roles = user.Roles
            // Add all user roles as separate claims
            foreach (var role in user.Roles.Select(r => r.Role?.RoleName))
            {
                if (!string.IsNullOrEmpty(role.ToString()))
                    authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            // --- 2. Access Token ---
            var jwtKey = _config["JWT:Key"] ?? throw new InvalidOperationException("JWT Key is missing");
            var jwtIssuer = _config["JWT:Issuer"] ?? "defaultIssuer";
            var jwtAudience = _config["JWT:Audience"] ?? "defaultAudience";
            var validationDays = int.TryParse(_config["JWT:validationDay"], out var days) ? days : 7;

            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddDays(validationDays);

            var jwtToken = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                expires: expires,
                claims: authClaims,
                signingCredentials: credentials
            );

            string accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            // --- 3. Refresh Token ---
            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Revoked = false,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(10) // configurable too
            };

            // --- 4. Cleanup old tokens ---
            var oldTokens = _RRepo.GetAll()
                                  .Where(rt => rt.UserId == user.Id)
                                  .Where(rt => rt.ExpiresAt <= DateTime.UtcNow || rt.Revoked)
                                  .ToList();

            foreach (var item in oldTokens)
                await _WRepo.DeleteAsync(item.Id);

            // --- 5. Save new refresh token ---
            await _Uow.BeginTransactionAsync();
            await _WRepo.AddAsync(refreshTokenEntity);
            await _Uow.SaveChangesAsync();
            await _Uow.CommitAsync();

            return (accessToken, refreshToken, jwtToken.ValidTo);
        }


        // --- Refresh Token Flow ---
        public async Task<(string AccessToken, string RefreshToken, DateTime Expiration)?> RefreshTokenAsync(string refreshToken)
        {
            var refreshTokenEntity = await _RRepo.GetAll()
                                        .Include(r => r.User).ThenInclude(u=>u.Roles).ThenInclude(ur => ur.Role)
                                        .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (refreshTokenEntity == null || refreshTokenEntity.ExpiresAt <= DateTime.UtcNow)
                return null;

            // Invalidate the old refresh token
            refreshTokenEntity.Revoked = true;
            await _WRepo.UpdateAsync(refreshTokenEntity, refreshTokenEntity.Id);
            await _Uow.BeginTransactionAsync();
            await _Uow.SaveChangesAsync();
            await _Uow.CommitAsync();


            // Issue new token for the user
            return await CreateTokenAsync(refreshTokenEntity.User);
        }

        // Generate Access + Refresh token
        // Helper
        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
        // Extract user from JWT
        // Extract user from Access Token
        public async Task<User?> GetUserFromTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["JWT:AuthKey"]);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _config["JWT:ValidIssuer"],
                    ValidAudience = _config["JWT:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // no leeway in expiry
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                if (validatedToken == null) return null;

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId)) return null;

                return await _RUR.GetByIdAsync(userId);
            }
            catch
            {
                return null;
            }
        }


    }

    //public class Authentication : IAuthentication
    //{

    //    private readonly IConfiguration _config;
    //    private IWriteGenericRepo<RefreshToken> _WRepo;
    //    private IReadGenericRepo<RefreshToken> _RRepo;
    //    private IWriteUnitOfWork _Uow;

    //    public Authentication(IConfiguration config, IWriteGenericRepo<RefreshToken> wRepo, IReadGenericRepo<RefreshToken> rRepo, IWriteUnitOfWork uow)
    //    {
    //        _config = config;
    //        _WRepo = wRepo;
    //        _RRepo = rRepo;
    //        _Uow = uow;
    //    }

    //    //IAuthentication<>


    //    // Generate Access + Refresh token
    //    public async Task<(string AccessToken, string RefreshToken, DateTime Expiration)> CreateTokenAsync<T>(T user) where T : User
    //    {
    //        await _Uow.BeginTransiaction();

    //        // --- 1. Build Claims (null-safe) ---
    //        var authClaims = new List<Claim>
    //        {
    //            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    //            new Claim(ClaimTypes.Name, $"{user.FName?? string.Empty} {user.LName?? string.Empty}"),
    //            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
    //            new Claim(ClaimTypes.Role, user.GetType().Name ?? "User")
    //        };

    //        // --- 2. Create Access Token ---
    //        var keyBytes = Encoding.UTF8.GetBytes(_config["JWT:Key"]); // FIXED
    //        var securityKey = new SymmetricSecurityKey(keyBytes);
    //        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    //        var jwtToken = new JwtSecurityToken(
    //            issuer: _config["JWT:Issuer"],
    //            audience: _config["JWT:Audience"],
    //            expires: DateTime.UtcNow.AddMinutes(5),
    //            claims: authClaims,
    //            signingCredentials: credentials
    //        );

    //        string accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

    //        // --- 3. Create Refresh Token ---
    //        var refreshToken = GenerateRefreshToken();
    //        var refreshTokenEntity = new RefreshToken
    //        {
    //            TravelerId = user is Traveler ? user.Id : null,
    //            TourGuideId = user is TourGuide ? user.Id : null,
    //            TravelCompanyId = user is TravelCompany ? user.Id : null,
    //            Revoked = false,
    //            Token = refreshToken,
    //            ExpiresAt = DateTime.UtcNow.AddDays(7)
    //        };

    //        // --- 4. Remove old tokens ---
    //        var oldTokens = _RRepo.GetAll()
    //        .Where(rt =>
    //            (typeof(T) == typeof(Traveler) && rt.TravelerId == user.Id) ||
    //            (typeof(T) == typeof(TourGuide) && rt.TourGuideId == user.Id) ||
    //            (typeof(T) == typeof(TravelCompany) && rt.TravelCompanyId == user.Id) ||
    //            (typeof(T) == typeof(Admin) && rt.AdminId == user.Id))
    //        .Where(rt => rt.ExpiresAt <= DateTime.UtcNow || rt.Revoked)
    //        .ToList();

    //        if (oldTokens.Any())
    //        {
    //            foreach (var item in oldTokens)
    //            {
    //                await _WRepo.DeleteAsync(item.Id);
    //            }
    //        }

    //        // --- 5. Add new refresh token ---
    //        //user.RefreshTokens.Add(refreshTokenEntity);

    //        await _WRepo.AddAsync(refreshTokenEntity);

    //        await _Uow.SaveChangesAsync();

    //        return (accessToken, refreshToken, jwtToken.ValidTo);
    //    }

    //    private string GenerateRefreshToken()
    //    {
    //        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    //    }

    //}

}
