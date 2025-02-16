using Auth.Api.Rest.Interfaces;
using Auth.Application.DTOs;
using Auth.Application.DTOs.Results;
using Auth.Application.Interfaces;
using Auth.Domain.Models;
using Auth.Domain.Models.Users;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Rest.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService, ITokenService tokenService) : ControllerBase
{
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

    private void SetRefreshTokenCookie(string refreshToken)
    {
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(30),
        };

        Response.Cookies.Append("refresh_token", refreshToken, cookieOptions);
    }
}