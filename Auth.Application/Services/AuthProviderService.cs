using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Domain.Constants;
using Auth.Domain.Models;
using Auth.Domain.Repositories;
using FluentResults;

namespace Auth.Application.Services;

public sealed class AuthProviderService(IAuthRepository authRepository) : IAuthProviderService
{
    public async Task<Result<User>> FindProviderUser(ProviderDto provider, string providerName)
    {
        try
        {
            Result<User> result = await authRepository.FindUser(provider.UserEmail);

            if (result.IsFailed)
            {
                Result<User> addResult = await AddProviderUser(provider, providerName);

                return addResult.IsFailed ? Result.Fail(addResult.Errors) : addResult;
            }

            if (result.Value.Providers.Any(p => p.Name == providerName))
            {
                return result;
            }

            Result<User> updateResult = await UpdateProviderUser(result.Value, providerName);

            return updateResult.IsSuccess ? Result.Ok(result.Value) : Result.Fail(updateResult.Errors);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    private async Task<Result<User>> AddProviderUser(ProviderDto provider, string providerName)
    {
        try
        {
            Result<OauthProvider> oauthProvider = await authRepository.GetOauthProvider(providerName);

            if (oauthProvider.IsFailed)
            {
                return Result.Fail(oauthProvider.Errors);
            }

            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = provider.Username,
                UserEmail = provider.UserEmail,
                CreatedDate = DateTime.Now,
                Providers = new List<OauthProvider> { oauthProvider.Value },
            };

            Result<User> result = await authRepository.InsertUser(user);
            return Result.Ok(result.Value);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    private async Task<Result<User>> UpdateProviderUser(User user, string providerName)
    {
        try
        {
            Result<OauthProvider> oauthProvider = await authRepository.GetOauthProvider(providerName);

            if (oauthProvider.IsFailed)
            {
                return Result.Fail(oauthProvider.Errors);
            }

            user.Providers.Add(oauthProvider.Value);

            Result<User> result = await authRepository.UpdateUser(user);
            return Result.Ok(result.Value);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }
}