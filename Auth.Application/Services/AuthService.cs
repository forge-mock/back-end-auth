using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Application.Services.Validators;
using Auth.Domain.Models;
using Auth.Domain.Repositories;
using FluentResults;
using Shared.Constants;
using Shared.Models;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Auth.Application.Services;

public sealed class AuthService(IAuthRepository authRepository) : IAuthService
{
    public async Task<Result<UserIdentify>> Authenticate(LoginDto login)
    {
        try
        {
            LoginDtoValidator validator = new();
            ValidationResult validationResult = await validator.ValidateAsync(login);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result.Fail(errors);
            }

            Result<UserIdentify> result = await authRepository.IdentifyUser(login.UserEmail);

            if (result.IsFailed)
            {
                return result;
            }

            bool isPasswordValid = PasswordHasher.Verify(login.Password, result.Value.Password);

            return isPasswordValid ? Result.Ok(result.Value) : Result.Fail("Username or password is incorrect");
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
            RegisterDtoValidator validator = new();
            ValidationResult validationResult = await validator.ValidateAsync(register);

            if (!validationResult.IsValid)
            {
                List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result.Fail(errors);
            }

            Result<bool> isUserExists = await authRepository.CheckIsUserExists(register.UserEmail);

            if (isUserExists.Value)
            {
                return Result.Fail("User already exists. Please, change email");
            }

            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = register.Username,
                UserEmail = register.UserEmail,
                Password = PasswordHasher.Hash(register.Password),
                CreatedDate = DateTime.UtcNow,
            };

            Result<User> result = await authRepository.InsertUser(user);

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

            if (savedRefreshToken.IsFailed || savedRefreshToken.Value.ExpirationDate < DateTime.UtcNow)
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

    public async Task<Result<bool>> Logout(Guid userId)
    {
        try
        {
            Result<RefreshToken> refreshToken = await authRepository.GetRefreshToken(userId);

            if (refreshToken.IsFailed)
            {
                return Result.Fail("Token does not exist");
            }

            Result<bool> result = authRepository.RemoveRefreshToken(userId);

            return result;
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }
}