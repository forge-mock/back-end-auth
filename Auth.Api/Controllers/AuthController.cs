using Auth.Api.Constants;
using Auth.Api.Interfaces;
using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Shared.Models;

namespace Auth.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAntiforgery antiforgery, IAuthService authService, ITokenService tokenService)
    : BaseAuthController(tokenService, authService)
{
    private readonly IAuthService authService = authService;
    private readonly ITokenService tokenService = tokenService;

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] LoginDto login)
    {
        Result<UserIdentify> result = await authService.Authenticate(login);

        if (result.IsFailed)
        {
            return BadRequest(new ResultFailDto(result.IsSuccess, result.Errors));
        }

        return await GenerateToken(result.Value, true);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto register)
    {
        string refreshToken = tokenService.GenerateRefreshToken();
        Result<Token> result = await authService.Register(register, refreshToken);

        if (result.IsFailed)
        {
            return BadRequest(new ResultFailDto(result.IsSuccess, result.Errors));
        }

        UserIdentify user = new(result.Value.UserId, register.Username, register.UserEmail, register.Password);

        return await GenerateToken(user, true);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string token)
    {
        try
        {
            await antiforgery.ValidateRequestAsync(HttpContext);
        }
        catch (AntiforgeryValidationException)
        {
            return BadRequest("Invalid CSRF token");
        }

        string? refreshToken = Request.Cookies[Cookies.RefreshToken];
        Result<Dictionary<string, string>> validateResult =
            await tokenService.ValidateToken(token, refreshToken ?? string.Empty);

        if (validateResult.IsFailed)
        {
            return Unauthorized(new ResultFailDto(validateResult.IsSuccess, validateResult.Errors));
        }

        Guid userId = Guid.Parse(validateResult.Value[JwtRegisteredClaimNames.Sub]);
        string username = validateResult.Value[JwtRegisteredClaimNames.Name];
        string userEmail = validateResult.Value[JwtRegisteredClaimNames.Email];

        Result<bool> validateRefreshTokenResult =
            await authService.ValidateRefreshToken(userId, refreshToken ?? string.Empty);

        if (!validateRefreshTokenResult.Value)
        {
            return BadRequest(new ResultFailDto(false, validateRefreshTokenResult.Errors));
        }

        UserIdentify user = new(userId, username, userEmail, string.Empty);

        return await GenerateToken(user, true);
    }

    [HttpGet("clear-refresh-token")]
    public NoContentResult ClearRefreshToken()
    {
        Response.Cookies.Append(
            Cookies.RefreshToken,
            string.Empty,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
            });

        return NoContent();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] string token)
    {
        try
        {
            await antiforgery.ValidateRequestAsync(HttpContext);
        }
        catch (AntiforgeryValidationException)
        {
            return BadRequest("Invalid CSRF token");
        }

        string? refreshToken = Request.Cookies[Cookies.RefreshToken];
        Result<Dictionary<string, string>> validateResult =
            await tokenService.ValidateToken(token, refreshToken ?? string.Empty);

        if (validateResult.IsFailed)
        {
            return Unauthorized(new ResultFailDto(validateResult.IsSuccess, validateResult.Errors));
        }

        Guid userId = Guid.Parse(validateResult.Value[JwtRegisteredClaimNames.Sub]);

        Result<bool> result = await authService.Logout(userId);

        if (result.IsFailed)
        {
            return BadRequest(new ResultFailDto(result.IsSuccess, result.Errors));
        }

        return Ok(new ResultSuccessDto<bool>(result.IsSuccess, result.Value));
    }
}