using Auth.Api.Rest.Interfaces;
using Auth.Api.Rest.Services;
using Auth.Application.Interfaces;
using Auth.Application.Services;
using Auth.Domain.Repositories;
using Auth.Persistence.Repositories;

namespace Auth.Api.Rest.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
    }

    internal static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAuthRepository, AuthRepository>();
    }

    internal static void AddApiRestServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<IMiddlewareService, MiddlewareService>();
    }
}