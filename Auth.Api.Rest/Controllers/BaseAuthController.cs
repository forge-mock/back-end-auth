using Auth.Api.Rest.Interfaces;
using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Domain.Models;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Rest.Controllers;

public class BaseAuthController(ITokenService tokenService, IAuthService authService) : ControllerBase
{
    protected async Task<IActionResult> GenerateToken(UserIdentify user)
    {
        Result<string> tokenResult = tokenService.GenerateToken(user);

        if (tokenResult.IsFailed)
        {
            return BadRequest(new ResultFailDto(tokenResult.IsSuccess, tokenResult.Errors));
        }

        string refreshToken = tokenService.GenerateRefreshToken();
        Result<string> refreshTokenResult = await authService.RefreshToken(user.Id, refreshToken);

        if (refreshTokenResult.IsFailed)
        {
            return BadRequest(new ResultFailDto(refreshTokenResult.IsSuccess, refreshTokenResult.Errors));
        }

        SetRefreshTokenCookie(refreshTokenResult.Value);

        return Ok(new ResultSuccessDto<string>(tokenResult.IsSuccess, tokenResult.Value));
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        CookieOptions cookieOptions = tokenService.GetRefreshTokenCookieOptions();
        Response.Cookies.Append("refresh_token", refreshToken, cookieOptions);
    }
}