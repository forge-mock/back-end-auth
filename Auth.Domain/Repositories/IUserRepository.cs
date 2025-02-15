using Auth.Domain.Models;

namespace Auth.Domain.Repositories;

public interface IUserRepository
{
    public Task<User> IdentifyUser(string username, string password);

    public Task<User> RegisterUser(User user);

    public Task<Token> RefreshToken(Guid userId);
}