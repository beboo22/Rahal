using Domain.Entity.Identity;

namespace Application.Abestraction
{
    public interface IAuthentication
    {
        Task<(string AccessToken, string RefreshToken, DateTime Expiration)> CreateTokenAsync(User user);

        // Extract user from JWT
        Task<User> GetUserFromTokenAsync(string token);

        // Refresh token flow
        //Task<(string AccessToken, string RefreshToken, DateTime Expiration)?> RefreshTokenAsync(string refreshToken);
        Task<(string AccessToken, string RefreshToken, DateTime Expiration)?> RefreshTokenAsync(string refreshToken);
    }
    //public interface IAuthentication
    //{
    //    // Refresh token flow
    //}
}
