using Auth.Api.Rest.Interfaces;
using Auth.Application.DTOs;
using Auth.Application.DTOs.Results;
using Auth.Application.Interfaces;
using Auth.Domain.Models.Tokens;
using Auth.Domain.Models.Users;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Auth.Api.Rest.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService, ITokenService tokenService) : ControllerBase
{
    private const string RefreshTokenCookie = "refresh_token";

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] LoginDto login)
    {
        Result<UserIdentify> result = await authService.Authenticate(login);

        if (result.IsFailed)
        {
            return BadRequest(new ResultFailDto(result.IsSuccess, result.Errors));
        }

        Result<string> tokenResult = tokenService.GenerateToken(result.Value);

        if (tokenResult.IsFailed)
        {
            return BadRequest(new ResultFailDto(tokenResult.IsSuccess, tokenResult.Errors));
        }

        string refreshToken = tokenService.GenerateRefreshToken();
        Result<string> refreshTokenResult = await authService.RefreshToken(result.Value.Id, refreshToken);

        if (refreshTokenResult.IsFailed)
        {
            return BadRequest(new ResultFailDto(refreshTokenResult.IsSuccess, refreshTokenResult.Errors));
        }

        SetRefreshTokenCookie(refreshTokenResult.Value);

        return Ok(new ResultSuccessDto<string>(tokenResult.IsSuccess, tokenResult.Value));
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
        Result<string> tokenResult = tokenService.GenerateToken(user);

        if (tokenResult.IsFailed)
        {
            return BadRequest(new ResultFailDto(tokenResult.IsSuccess, tokenResult.Errors));
        }

        SetRefreshTokenCookie(refreshToken);

        return Ok(new ResultSuccessDto<string>(result.IsSuccess, tokenResult.Value));
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string token)
    {
        string? refreshToken = Request.Cookies[RefreshTokenCookie];
        Result<Dictionary<string, string>> validateResult = await tokenService.ValidateToken(token, refreshToken);

        if (validateResult.IsFailed)
        {
            return Unauthorized(new ResultFailDto(validateResult.IsSuccess, validateResult.Errors));
        }

        Guid userId = Guid.Parse(validateResult.Value[JwtRegisteredClaimNames.Sub]);
        string username = validateResult.Value[JwtRegisteredClaimNames.Name];
        string userEmail = validateResult.Value[JwtRegisteredClaimNames.Email];

        Result<bool> validateRefreshTokenResult = await authService.ValidateRefreshToken(userId, refreshToken);

        if (!validateRefreshTokenResult.Value)
        {
            return BadRequest(new ResultFailDto(false, validateRefreshTokenResult.Errors));
        }

        string updatedRefreshToken = tokenService.GenerateRefreshToken();
        Result<string> result = await authService.RefreshToken(userId, updatedRefreshToken);

        if (result.IsFailed)
        {
            return BadRequest(new ResultFailDto(result.IsSuccess, result.Errors));
        }

        UserIdentify user = new(userId, username, userEmail, string.Empty);
        Result<string> tokenResult = tokenService.GenerateToken(user);

        if (tokenResult.IsFailed)
        {
            return BadRequest(new ResultFailDto(tokenResult.IsSuccess, tokenResult.Errors));
        }

        SetRefreshTokenCookie(updatedRefreshToken);

        return Ok(new ResultSuccessDto<string>(result.IsSuccess, tokenResult.Value));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] string token)
    {
        string? refreshToken = Request.Cookies[RefreshTokenCookie];
        Result<Dictionary<string, string>> validateResult = await tokenService.ValidateToken(token, refreshToken);

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

    private void SetRefreshTokenCookie(string refreshToken)
    {
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(30),
        };

        Response.Cookies.Append(RefreshTokenCookie, refreshToken, cookieOptions);
    }
}