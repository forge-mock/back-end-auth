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
            Result<User> result = await authRepository.FindUserWithProvider(provider.UserEmail);

            if (result.Value.UserOauthProviders.Any(p => p.ProviderAccountId == provider.ProviderAccountId))
            {
                return result;
            }

            if (result.IsFailed)
            {
                Result<User> addResult = await AddProviderUser(provider, providerName);

                return addResult.IsFailed ? Result.Fail(addResult.Errors) : addResult;
            }

            if (result.Value.UserOauthProviders.Any(p => p.Provider.Name == providerName))
            {
                return result;
            }

            Result<User> updateResult = await UpdateProviderUser(result.Value, provider, providerName);

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

            Guid newUserId = Guid.NewGuid();

            User user = new()
            {
                Id = newUserId,
                Username = provider.Username,
                UserEmail = provider.UserEmail,
                CreatedDate = DateTime.Now,
                UserOauthProviders = new List<UserOauthProvider>
                {
                    new()
                    {
                        UserId = newUserId,
                        Provider = oauthProvider.Value,
                        ProviderId = oauthProvider.Value.Id,
                        ProviderAccountId = provider.ProviderAccountId,
                    },
                },
            };

            Result<User> result = await authRepository.InsertUser(user);
            return Result.Ok(result.Value);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    private async Task<Result<User>> UpdateProviderUser(User user, ProviderDto provider, string providerName)
    {
        try
        {
            Result<OauthProvider> oauthProvider = await authRepository.GetOauthProvider(providerName);

            if (oauthProvider.IsFailed)
            {
                return Result.Fail(oauthProvider.Errors);
            }

            UserOauthProvider userProvider = new()
            {
                UserId = user.Id,
                Provider = oauthProvider.Value,
                ProviderId = oauthProvider.Value.Id,
                ProviderAccountId = provider.ProviderAccountId,
            };

            Result<User> result = await authRepository.UpdateUserProvider(user, userProvider);
            return Result.Ok(result.Value);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }
}