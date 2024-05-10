using System.IdentityModel.Tokens.Jwt;
using AuthManager.DataTransferObjects;

namespace AuthManager.Abstractions;

public interface IJwtUtilities
{
    public TokensDto GenerateJwtTokens(int uid, string email, string username, DateTime createdAt, bool isVerified,
        bool isAdmin, string secretKey, TimeSpan? accessTokenLifeSpan = null, TimeSpan? refreshTokenLifeSpan = null);

    public JwtSecurityToken? ValidateJwtToken(string token, string secretKey);
    
    public JwtSecurityToken ReadJwtToken(string token, string secretKey);
}