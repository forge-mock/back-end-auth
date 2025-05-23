using Auth.Domain.Models;
using FluentResults;

namespace Auth.Api.Interfaces;

public interface ITokenService
{
    public Result<string> GenerateToken(UserIdentify user);

    public Task<Result<Dictionary<string, string>>> ValidateToken(string token, string refreshToken);

    public string GenerateRefreshToken();

    public CookieOptions GetRefreshTokenCookieOptions();
}