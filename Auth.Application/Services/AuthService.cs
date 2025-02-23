using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Domain.Constants;
using Auth.Domain.Models.Tokens;
using Auth.Domain.Models.Users;
using Auth.Domain.Repositories;
using FluentResults;

namespace Auth.Application.Services;

public sealed class AuthService(IAuthRepository authRepository) : IAuthService
{
    public async Task<Result<UserIdentify>> Authenticate(LoginDto login)
    {
        try
        {
            if (string.IsNullOrEmpty(login.UserInput) || string.IsNullOrEmpty(login.Password))
            {
                return Result.Fail("Username, user email or password are empty");
            }

            Result<UserIdentify> result = await authRepository.IdentifyUser(login.UserInput, login.Password);

            if (result.IsFailed)
            {
                return result;
            }

            bool isUserValid = result.Value.Password == login.Password;

            return isUserValid ? Result.Ok(result.Value) : Result.Fail("Username or password is incorrect");
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    public async Task<Result<Token>> Register(RegisterDto register, string refreshToken)
    {
        try
        {
            if (string.IsNullOrEmpty(register.UserEmail) ||
                string.IsNullOrEmpty(register.Username) ||
                string.IsNullOrEmpty(register.Password))
            {
                return Result.Fail("Username, user email and password should not be empty");
            }

            Result<bool> isUserExists = await authRepository.CheckIsUserExists(register.Username, register.UserEmail);

            if (isUserExists.Value)
            {
                return Result.Fail("User already exists");
            }

            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = register.Username,
                UserEmail = register.UserEmail,
                Password = register.Password,
                CreatedDate = DateTime.UtcNow,
            };

            Result<User> result = await authRepository.RegisterUser(user);

            if (result.IsFailed)
            {
                return Result.Fail(result.Errors);
            }

            Token token = new()
            {
                Id = Guid.NewGuid(),
                Name = refreshToken,
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                UserId = user.Id,
            };

            Result<Token> tokenResult = await authRepository.CreateRefreshToken(token);

            return Result.Ok(tokenResult.Value);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    public async Task<Result<bool>> ValidateRefreshToken(Guid userId, string refreshToken)
    {
        try
        {
            Result<RefreshToken> savedRefreshToken = await authRepository.GetRefreshToken(userId);

            if (savedRefreshToken.IsFailed)
            {
                return Result.Fail("Please, login again!");
            }

            bool validateToken = savedRefreshToken.Value.Name == refreshToken;

            return Result.Ok(validateToken);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }

    public async Task<Result<string>> RefreshToken(Guid userId, string refreshToken)
    {
        try
        {
            Token token = new()
            {
                Name = refreshToken,
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                UserId = userId,
            };

            Result<RefreshToken> savedRefreshToken = await authRepository.GetRefreshToken(userId);

            if (savedRefreshToken.IsFailed)
            {
                token.Id = Guid.NewGuid();
                Result<Token> result = await authRepository.CreateRefreshToken(token);
                return Result.Ok(result.Value.Name);
            }

            token.Id = savedRefreshToken.Value.Id;
            Result<Token> updateResult = await authRepository.UpdateRefreshToken(token);
            return Result.Ok(updateResult.Value.Name);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }
}