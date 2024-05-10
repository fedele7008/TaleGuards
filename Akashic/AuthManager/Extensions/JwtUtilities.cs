using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthManager.Abstractions;
using AuthManager.DataTransferObjects;
using Microsoft.IdentityModel.Tokens;

namespace AuthManager.Extensions;

public class JwtUtilities : IJwtUtilities
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();
    private static readonly SHA256 Sha256HashConverter = SHA256.Create();
    public TokensDto GenerateJwtTokens(int uid, string email, string username, DateTime createdAt, bool isVerified, bool isAdmin,
        string secretKey, TimeSpan? accessTokenLifeSpan = null, TimeSpan? refreshTokenLifeSpan = null)
    {
        accessTokenLifeSpan ??= TimeSpan.FromMinutes(10);
        refreshTokenLifeSpan ??= TimeSpan.FromDays(30);
        
        var claims = new List<Claim>
        {
            new ("uid", uid.ToString()),
            new ("email", email),
            new ("username", username),
            new ("createdAt", createdAt.ToString(CultureInfo.InvariantCulture)),
            new ("isVerified", isVerified.ToString()),
            new ("isAdmin", isAdmin.ToString())
        };
        
        var claimsIdentity = new ClaimsIdentity(claims, "UserToken");

        var sha256SecretKey = Sha256HashConverter.ComputeHash(Encoding.UTF8.GetBytes(secretKey));
        
        var accessTokenSecurityDescriptor = new SecurityTokenDescriptor()
        {
            Subject = claimsIdentity,
            Issuer = "Akashic",
            Audience = email,
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.Add((TimeSpan)accessTokenLifeSpan),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(sha256SecretKey),
                SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest)
        };
        
        var refreshTokenSecurityDescriptor = new SecurityTokenDescriptor()
        {
            Subject = claimsIdentity,
            Issuer = "Akashic",
            Audience = email,
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.Add((TimeSpan)refreshTokenLifeSpan),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(sha256SecretKey),
                SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest)
        };

        var accessToken = TokenHandler.WriteToken(TokenHandler.CreateToken(accessTokenSecurityDescriptor));
        var refreshToken = TokenHandler.WriteToken(TokenHandler.CreateToken(refreshTokenSecurityDescriptor));

        return new TokensDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }

    public JwtSecurityToken? ValidateJwtToken(string token, string secretKey)
    {
        var sha256SecretKey = Sha256HashConverter.ComputeHash(Encoding.UTF8.GetBytes(secretKey));
        
        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(sha256SecretKey),
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidIssuer = "Akashic"
        };
        
        TokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

        return validatedToken as JwtSecurityToken;
    }

    public JwtSecurityToken ReadJwtToken(string token, string secretKey)
    {
        var tokenWithoutValidation = TokenHandler.ReadToken(token);
        return tokenWithoutValidation as JwtSecurityToken ?? throw new Exception("Failed to read JWT token");
    }
}