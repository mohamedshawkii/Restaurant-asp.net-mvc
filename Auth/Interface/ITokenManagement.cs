using System.Security.Claims;

namespace Restaurant.Auth.Interface;

public interface ITokenManagement
{
    string GetRefreshTokens();
    List<Claim> GetUserClaimFromToken(string token);
    Task<bool> ValidationRefreshToken(string refreshToken);
    Task<string> GetUserIdByRefreshToken(string refreshToken);
    Task<int> AddRefreshToken(string userId, string refreshToken);
    Task<int> UpdateRefreshToken(string userId, string refreshToken);
    string GenerateToken(List<Claim> claims);
}
