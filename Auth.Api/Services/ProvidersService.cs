using Auth.Api.Rest.Interfaces;
using FluentResults;
using Google.Apis.Auth;

namespace Auth.Api.Rest.Services;

public sealed class ProvidersService : IProvidersService
{
    private static readonly HttpClient Client = new();

    public async Task<Result<GoogleJsonWebSignature.Payload>> VerifyGoogleToken(string token)
    {
        try
        {
            GoogleJsonWebSignature.Payload? payload = await GoogleJsonWebSignature.ValidateAsync(
                token,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience =
                    [
                        Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID"),
                    ],
                });

            return Result.Ok(payload);
        }
        catch
        {
            return Result.Fail("Google token is invalid");
        }
    }

    public async Task<Result<bool>> VerifyGitHubToken(string token)
    {
        try
        {
            HttpRequestMessage request = new(HttpMethod.Get, "https://api.github.com/user");
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("User-Agent", "Forge Mock");

            HttpResponseMessage response = await Client.SendAsync(request);

            return Result.Ok(response.IsSuccessStatusCode);
        }
        catch
        {
            return Result.Fail("GitHub token is invalid");
        }
    }
}