using Auth.Domain.Models.Users;
using FluentResults;

namespace Auth.Api.Rest.Interfaces;

public interface ITokenService
{
    public Result<string> GenerateToken(UserIdentify user);
}