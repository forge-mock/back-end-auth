using Auth.Application.DTOs;
using Auth.Domain.Models;
using FluentResults;

namespace Auth.Application.Interfaces;

public interface IAuthService
{
    public Task<Result<UserIdentify>> Authenticate(LoginDto login);

    public Task<Result<Token>> Register(RegisterDto register, string refreshToken);

    public Task<Result<bool>> ValidateRefreshToken(Guid userId, string refreshToken);

    public Task<Result<string>> RefreshToken(Guid userId, string refreshToken);

    public Task<Result<bool>> Logout(Guid userId);
}