using Auth.Domain.Constants;
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
                .Select(u => new UserIdentify(u.Id, u.Username, u.UserEmail, u.Password))
                .FirstOrDefaultAsync();

            return user == null ? Result.Fail("User does not exist") : Result.Ok(user);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    public async Task<Result<bool>> CheckIsUserExists(string username, string userEmail)
    {
        try
        {
            bool userExists = await context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Username == username || u.UserEmail == userEmail);

            return Result.Ok(userExists);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    public async Task<Result<User>> RegisterUser(User user)
    {
        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return Result.Ok(user);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    public async Task<Result<Guid>> GetRefreshToken(Guid userId)
    {
        try
        {
            Guid tokenId = await context.Tokens
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .Select(t => t.Id)
                .FirstOrDefaultAsync();

            return tokenId == Guid.Empty ? Result.Fail("Token does not exist") : Result.Ok(tokenId);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    public async Task<Result<Token>> CreateRefreshToken(Token token)
    {
        try
        {
            await context.Tokens.AddAsync(token);
            await context.SaveChangesAsync();
            return Result.Ok(token);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    public async Task<Result<Token>> UpdateRefreshToken(Token token)
    {
        try
        {
            context.Tokens.Update(token);
            await context.SaveChangesAsync();
            return Result.Ok(token);
        }
        catch (Exception ex)
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }
}