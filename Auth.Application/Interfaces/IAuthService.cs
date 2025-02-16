using Auth.Application.DTOs;
using Auth.Domain.Models.Users;
using FluentResults;

namespace Auth.Application.Interfaces;

public interface IAuthService
{
    public Task<Result<UserIdentify>> Authenticate(LoginDto login);

    public Task<Result<string>> Register(RegisterDto register, string refreshToken);

    public Task<Result<string>> RefreshToken(string refreshToken);
}