using FluentResults;
using Google.Apis.Auth;

namespace Auth.Api.Interfaces;

public interface IProvidersService
{
    public Task<Result<GoogleJsonWebSignature.Payload>> VerifyGoogleToken(string token);
    public Task<Result<bool>> VerifyGitHubToken(string token);
}