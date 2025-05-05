using Auth.Domain.Models;
using FluentResults;

namespace Auth.Domain.Repositories;

public interface IAuthRepository
{
    public Task<Result<UserIdentify>> IdentifyUser(string userEmail);

    public Task<Result<User>> FindUserWithProvider(string userEmail);

    public Task<Result<User>> InsertUser(User user);

    public Task<Result<bool>> CheckIsUserExists(string userEmail);

    public Task<Result<RefreshToken>> GetRefreshToken(Guid userId);

    public Task<Result<Token>> CreateRefreshToken(Token token);

    public Task<Result<Token>> UpdateRefreshToken(Token token);

    public Result<bool> RemoveRefreshToken(Guid userId);

    public Task<Result<OauthProvider>> GetOauthProvider(string name);

    public Task<Result<User>> UpdateUserProvider(User user, UserOauthProvider provider);
}