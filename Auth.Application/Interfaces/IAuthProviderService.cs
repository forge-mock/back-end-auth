using Auth.Application.DTOs;
using Auth.Domain.Models;
using FluentResults;

namespace Auth.Application.Interfaces;

public interface IAuthProviderService
{
    public Task<Result<User>> FindProviderUser(ProviderDto provider, string providerName);
}