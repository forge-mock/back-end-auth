using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Auth.Api.Rest.Interfaces;
using Auth.Domain.Constants;
using Auth.Domain.Models;
using FluentResults;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Api.Rest.Services;

public sealed class TokenService(IConfiguration configuration) : ITokenService
{
    private const string AuthErrorMessage = "Auth error occurred. Please, contact our support!";
    private const string JwtSecretEnvironmentVariable = "JWT_SECRET";
    private const string ExpirationTime = "Jwt:ExpirationTime";
    private const string Issuer = "Jwt:Issuer";
    private const string Audience = "Jwt:Audience";

    public async Task<Result<Dictionary<string, string>>> ValidateToken(string token, string refreshToken)
    {
        try
        {
            string? secretKey = Environment.GetEnvironmentVariable(JwtSecretEnvironmentVariable);

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
            {
                return Result.Fail(AuthErrorMessage);
            }

            JsonWebTokenHandler tokenHandler = new();
            TokenValidationParameters validationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration[Issuer],
                ValidAudience = configuration[Audience],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            };

            TokenValidationResult? result = await tokenHandler.ValidateTokenAsync(token, validationParameters);

            if (!result.IsValid)
            {
                return Result.Fail("Invalid token");
            }

            JsonWebToken jsonToken = tokenHandler.ReadJsonWebToken(token);
            Dictionary<string, string> claims = jsonToken.Claims.ToDictionary(c => c.Type, c => c.Value);

            return Result.Ok(claims);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    public Result<string> GenerateToken(UserIdentify user)
    {
        try
        {
            string? secretKey = Environment.GetEnvironmentVariable(JwtSecretEnvironmentVariable);

            if (string.IsNullOrEmpty(secretKey))
            {
                return Result.Fail(AuthErrorMessage);
            }

            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Name, user.Username),
                    new Claim(JwtRegisteredClaimNames.Email, user.UserEmail),
                ]),
                Expires = DateTime.UtcNow
                    .AddMinutes(configuration.GetValue<int>(ExpirationTime)),
                SigningCredentials = credentials,
                Issuer = configuration[Issuer],
                Audience = configuration[Audience],
            };

            JsonWebTokenHandler tokenHandler = new();
            string jwtToken = tokenHandler.CreateToken(tokenDescriptor);

            return Result.Ok(jwtToken);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }
}