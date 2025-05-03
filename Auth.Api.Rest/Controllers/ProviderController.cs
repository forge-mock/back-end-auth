using Auth.Api.Rest.Interfaces;
using Auth.Application.DTOs;
using Auth.Application.DTOs.Results;
using FluentResults;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Rest.Controllers;

[ApiController]
[Route("provider")]
public class ProviderController(IProvidersService providersService) : ControllerBase
{
    [HttpPost("google")]
    public async Task<IActionResult> Google([FromBody] string token)
    {
        Result<GoogleJsonWebSignature.Payload> payloadResult = await providersService.VerifyGoogleToken(token);

        if (payloadResult.IsFailed)
        {
            return BadRequest(new ResultFailDto(payloadResult.IsSuccess, payloadResult.Errors));
        }

        // // Step 2: Token is valid - process the user info (you can store in DB)
        // var user = await _userRepository.FindOrCreateUserAsync(payload.Email, payload.Name);
        //
        // // Optionally, generate your own JWT here
        // var jwt = GenerateYourJwt(user);

        return Ok(new ResultSuccessDto<string>(payloadResult.IsSuccess, payloadResult.Value.Email));
    }

    [HttpPost("github")]
    public async Task<IActionResult> GitHub([FromBody] GitHubDto gitHubDto)
    {
        Result<bool> gitHubResult = await providersService.VerifyGitHubToken(gitHubDto.AccessToken);

        if (gitHubResult.IsFailed)
        {
            return BadRequest(new ResultFailDto(gitHubResult.IsSuccess, gitHubResult.Errors));
        }

        // // Step 2: Token is valid - process the user info (you can store in DB)
        // var user = await _userRepository.FindOrCreateUserAsync(payload.Email, payload.Name);
        //
        // // Optionally, generate your own JWT here
        // var jwt = GenerateYourJwt(user);

        return Ok(new ResultSuccessDto<string>(gitHubResult.IsSuccess, gitHubDto.AccessToken));
    }
}