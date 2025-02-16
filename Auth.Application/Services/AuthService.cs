using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Domain.Constants;
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

    public async Task<Result<User>> Register(RegisterDto register)
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

            return result.IsFailed ? result : Result.Ok(result.Value);
        }
        catch
        {
            return Result.Fail(ErrorMessage.Exception);
        }
    }
}