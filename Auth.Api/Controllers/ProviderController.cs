using Auth.Api.Constants;
using Auth.Api.Interfaces;
using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Domain.Models;
using FluentResults;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Auth.Api.Controllers;

[ApiController]
[Route("provider")]
public class ProviderController(
    IProvidersService providersService,
    ITokenService tokenService,
    IAuthProviderService authProviderService,
    IAuthService authService)
    : BaseAuthController(tokenService, authService)
{
    [HttpPost("google")]
    public async Task<IActionResult> Google([FromBody] ProviderDto provider)
    {
        Result<GoogleJsonWebSignature.Payload> payloadResult =
            await providersService.VerifyGoogleToken(provider.AccessToken);

        if (payloadResult.IsFailed)
        {
            return BadRequest(new ResultFailDto(payloadResult.IsSuccess, payloadResult.Errors));
        }

        return await BaseProvider(provider, Providers.Google);
    }

    [HttpPost("github")]
    public async Task<IActionResult> GitHub([FromBody] ProviderDto provider)
    {
        Result<bool> gitHubResult = await providersService.VerifyGitHubToken(provider.AccessToken);

        if (gitHubResult.IsFailed)
        {
            return BadRequest(new ResultFailDto(gitHubResult.IsSuccess, gitHubResult.Errors));
        }

        return await BaseProvider(provider, Providers.GitHub);
    }

    private async Task<IActionResult> BaseProvider(ProviderDto provider, string providerName)
    {
        Result<User> result = await authProviderService.FindProviderUser(provider, providerName);

        if (result.IsFailed)
        {
            return BadRequest(new ResultFailDto(result.IsSuccess, result.Errors));
        }

        UserIdentify user = new(result.Value.Id, result.Value.Username, result.Value.UserEmail, string.Empty);

        return await GenerateToken(user);
    }
}