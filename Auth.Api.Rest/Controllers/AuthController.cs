using Auth.Api.Rest.Constants;
using Auth.Api.Rest.Interfaces;
using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Auth.Api.Rest.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAntiforgery antiforgery, IAuthService authService, ITokenService tokenService)
    : BaseAuthController(tokenService, authService)
{
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] LoginDto login)
    {
        Result<UserIdentify> result = await authService.Authenticate(login);

        if (result.IsFailed)
        {
            return BadRequest(new ResultFailDto(result.IsSuccess, result.Errors));
        }

        return await GenerateToken(result.Value);
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

        return await GenerateToken(user);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string token)
    {
        await antiforgery.ValidateRequestAsync(HttpContext);

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

        return await GenerateToken(user);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] string token)
    {
        await antiforgery.ValidateRequestAsync(HttpContext);

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