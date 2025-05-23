using Auth.Domain.Models;
using Auth.Domain.Repositories;
using Auth.Persistence.Context;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.Models;

namespace Auth.Persistence.Repositories;

public sealed class AuthRepository(AuthContext context) : IAuthRepository
{
    public async Task<Result<UserIdentify>> IdentifyUser(string userEmail)
    {
        try
        {
            UserIdentify? user = await context.Users
                .AsNoTracking()
                .Where(u => u.UserEmail == userEmail)
                .Select(u => new UserIdentify(u.Id, u.Username, u.UserEmail, u.Password ?? string.Empty))
                .FirstOrDefaultAsync();

            return user == null ? Result.Fail("Username or password is incorrect") : Result.Ok(user);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    public async Task<Result<User>> FindUserWithProvider(string userEmail)
    {
        try
        {
            User? user = await context.Users
                .Include(u => u.UserOauthProviders)
                .ThenInclude(p => p.Provider)
                .FirstOrDefaultAsync(u => u.UserEmail == userEmail);

            return user == null ? Result.Fail("User does not exist") : Result.Ok(user);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    public async Task<Result<bool>> CheckIsUserExists(string userEmail)
    {
        try
        {
            bool userExists = await context.Users
                .AsNoTracking()
                .AnyAsync(u => u.UserEmail == userEmail);

            return Result.Ok(userExists);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    public async Task<Result<User>> InsertUser(User user)
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

    public async Task<Result<RefreshToken>> GetRefreshToken(Guid userId)
    {
        try
        {
            RefreshToken? token = await context.Tokens
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .Select(t => new RefreshToken(t.Id, t.Name, t.ExpirationDate))
                .FirstOrDefaultAsync();

            return token == null || token.Id == Guid.Empty ? Result.Fail("Token does not exist") : Result.Ok(token);
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
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    public Result<bool> RemoveRefreshToken(Guid userId)
    {
        context.Tokens.Where(t => t.UserId == userId).ExecuteDeleteAsync();
        return Result.Ok(true);
    }

    public async Task<Result<OauthProvider>> GetOauthProvider(string name)
    {
        try
        {
            OauthProvider? oauthProvider = await context.OauthProviders
                .AsNoTracking()
                .Where(op => op.Name == name)
                .FirstOrDefaultAsync();

            return oauthProvider == null || oauthProvider.Id == Guid.Empty
                ? Result.Fail("Provider does not exist")
                : Result.Ok(oauthProvider);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    public async Task<Result<User>> UpdateUserProvider(User user, UserOauthProvider provider)
    {
        try
        {
            context.Attach(provider.Provider);
            context.UserOauthProviders.Add(provider);
            await context.SaveChangesAsync();
            return Result.Ok(user);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }
}