using Auth.Application.DTOs;
using FluentResults;
using Shared.Models;

namespace Auth.Application.Interfaces;

public interface IAuthProviderService
{
    public Task<Result<User>> FindProviderUser(ProviderDto provider, string providerName);
}