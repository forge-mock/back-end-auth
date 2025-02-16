using Auth.Api.Rest.Interfaces;
using Auth.Application.DTOs;
using Auth.Application.DTOs.Results;
using Auth.Application.Interfaces;
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

        return Ok(new ResultSuccessDto<string>(tokenResult.IsSuccess, tokenResult.Value));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto register)
    {
        Result<User> result = await authService.Register(register);

        if (result.IsFailed)
        {
            return BadRequest(new ResultFailDto(result.IsSuccess, result.Errors));
        }

        return Ok(new ResultSuccessDto<User>(result.IsSuccess, result.Value));
    }
}