using Auth.Domain.Models;
using Auth.Domain.Models.Users;
using Auth.Domain.Repositories;
using Auth.Persistence.Context;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Auth.Persistence.Repositories;

public sealed class AuthRepository(AuthContext context) : IAuthRepository
{
    public async Task<Result<UserIdentify>> IdentifyUser(string userInput, string password)
    {
        try
        {
            UserIdentify? user = await context.Users
                .AsNoTracking()
                .Where(u => u.Username == userInput || u.UserEmail == userInput)
                .Select(u => new UserIdentify(u.Username, u.UserEmail, u.Password))
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Result.Fail("User does not exist");
            }

            bool isUserValid = user.Password == password;

            return isUserValid ? Result.Ok(user) : Result.Fail("Username or password is incorrect");
        }
        catch (Exception ex)
        {
            return Result.Fail($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Result<User>> RegisterUser(User user)
    {
        try
        {
            Result<bool> userExists = await CheckIsUserExists(user.Username, user.UserEmail);

            if (userExists.Value)
            {
                return Result.Fail("User already exists");
            }

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return Result.Ok(user);
        }
        catch (Exception ex)
        {
            return Result.Fail($"An error occurred: {ex.Message}");
        }
    }

    public Task<Token> RefreshToken(Guid userId)
    {
        throw new NotImplementedException();
    }

    private async Task<Result<bool>> CheckIsUserExists(string username, string userEmail)
    {
        try
        {
            bool userExists = await context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Username == username || u.UserEmail == userEmail);

            return Result.Ok(userExists);
        }
        catch (Exception ex)
        {
            return Result.Fail($"An error occurred: {ex.Message}");
        }
    }
}