using System.Security.Claims;
using System.Text;
using Auth.Api.Rest.Interfaces;
using Auth.Domain.Models.Users;
using FluentResults;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace Auth.Api.Rest.Services;

public sealed class TokenService(IConfiguration configuration) : ITokenService
{
    public Result<string> GenerateToken(UserIdentify user)
    {
        try
        {
            string? secretKey = Environment.GetEnvironmentVariable("JWT_SECRET");

            if (string.IsNullOrEmpty(secretKey))
            {
                return Result.Fail("Auth error occured. Please, connect to our support!");
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
                Expires = DateTime.Now.ToUniversalTime()
                    .AddMinutes(configuration.GetValue<int>("Jwt:ExpirationTime")),
                SigningCredentials = credentials,
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"],
            };

            JsonWebTokenHandler tokenHandler = new();
            string jwtToken = tokenHandler.CreateToken(tokenDescriptor);

            return Result.Ok(jwtToken);
        }
        catch
        {
            return Result.Fail("An unknown error occurred. Please, connect to our support!");
        }
    }
}