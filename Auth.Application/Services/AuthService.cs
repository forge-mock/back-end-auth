using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Application.Services.Validators;
using Auth.Domain.Constants;
using Auth.Domain.Models.Tokens;
using Auth.Domain.Models.Users;
using Auth.Domain.Repositories;
using FluentResults;
using Serilog;
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
                Log.Warning("Validation failed: {Errors}", string.Join(", ", errors));
                return Result.Fail(errors);
            }

            Result<UserIdentify> result = await authRepository.IdentifyUser(login.UserInput);

            if (result.IsFailed)
            {
                Log.Warning("Authentication failed: {Reason}", result.Errors[0].Message);
                return result;
            }

            bool isPasswordValid = PasswordHasher.Verify(login.Password, result.Value.Password);

            if (isPasswordValid)
            {
                Log.Information("User {UserId} authenticated successfully", result.Value.Id);
                return Result.Ok(result.Value);
            }

            Log.Warning("Authentication failed: Username or password is incorrect");
            return Result.Fail("Username or password is incorrect");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception occurred during authentication");
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
                Log.Warning("Validation failed: {Errors}", string.Join(", ", errors));
                return Result.Fail(errors);
            }

            Result<bool> isUserExists = await authRepository.CheckIsUserExists(register.Username, register.UserEmail);

            if (isUserExists.Value)
            {
                Log.Warning("Registration failed: User already exists");
                return Result.Fail("User already exists");
            }

            User user = new()
            {
                Id = Guid.NewGuid(),
                Username = register.Username,
                UserEmail = register.UserEmail,
                Password = PasswordHasher.Hash(register.Password),
                CreatedDate = DateTime.UtcNow,
            };

            Result<User> result = await authRepository.RegisterUser(user);

            if (result.IsFailed)
            {
                Log.Warning("Registration failed: {Reason}", result.Errors[0].Message);
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
            Log.Information("User {UserId} registered successfully", user.Id);
            return Result.Ok(tokenResult.Value);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception occurred during registration");
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
                Log.Warning("Validation failed: Refresh token not found for user or is has expired {UserId}", userId);
                return Result.Fail("Please, login again!");
            }

            bool validateToken = savedRefreshToken.Value.Name == refreshToken;

            Log.Information("Refresh token validated successfully for user {UserId}", userId);
            return Result.Ok(validateToken);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception occurred during refresh token validation for user {UserId}", userId);
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
                Log.Information("New refresh token created for user {UserId}", userId);
                return Result.Ok(result.Value.Name);
            }

            token.Id = savedRefreshToken.Value.Id;
            Result<Token> updateResult = await authRepository.UpdateRefreshToken(token);
            Log.Information("Refresh token updated successfully for user {UserId}", userId);
            return Result.Ok(updateResult.Value.Name);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception occurred during refreshing token for user {UserId}", userId);
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
                Log.Warning("Logout failed: Token does not exist for user {UserId}", userId);
                return Result.Fail("Token does not exist");
            }

            Result<bool> result = authRepository.RemoveRefreshToken(userId);
            Log.Information("User {UserId} logged out successfully", userId);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception occurred during logout for user {UserId}", userId);
            return Result.Fail(ErrorMessage.Exception);
        }
    }
}