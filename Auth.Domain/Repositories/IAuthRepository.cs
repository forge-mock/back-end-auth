using Auth.Domain.Models.Tokens;
using Auth.Domain.Models.Users;
using FluentResults;

namespace Auth.Domain.Repositories;

public interface IAuthRepository
{
    public Task<Result<UserIdentify>> IdentifyUser(string userInput, string password);

    public Task<Result<User>> RegisterUser(User user);

    public Task<Result<bool>> CheckIsUserExists(string username, string userEmail);

    public Task<Result<RefreshToken>> GetRefreshToken(Guid userId);

    public Task<Result<Token>> CreateRefreshToken(Token token);

    public Task<Result<Token>> UpdateRefreshToken(Token token);

    public Result<bool> RemoveRefreshToken(Guid userId);
}