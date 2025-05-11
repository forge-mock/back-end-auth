using Auth.Api.Interfaces;
using Auth.Api.Services;
using Auth.Application.Interfaces;
using Auth.Application.Services;
using Auth.Domain.Repositories;
using Auth.Persistence.Repositories;
using Shared.Interfaces;
using Shared.Services;

namespace Auth.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuthProviderService, AuthProviderService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
    }

    internal static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAuthRepository, AuthRepository>();
    }

    internal static void AddApiRestServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IProvidersService, ProvidersService>();
        services.AddSingleton<IMiddlewareService, MiddlewareService>();
    }
}