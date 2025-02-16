using Auth.Application.DTOs;
using Auth.Application.DTOs.Results;
using Auth.Application.Interfaces;
using Auth.Domain.Models.Users;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Rest.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] LoginDto login)
    {
        Result<UserIdentify> result = await authService.Authenticate(login);

        if (result.IsFailed)
        {
            return BadRequest(new ResultFailDto(result.IsSuccess, result.Errors));
        }

        return Ok(new ResultSuccessDto<UserIdentify>(result.IsSuccess, result.Value));
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