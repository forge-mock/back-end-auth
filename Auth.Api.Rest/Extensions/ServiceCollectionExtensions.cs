using Auth.Api.Rest.Interfaces;
using Auth.Api.Rest.Services;

namespace Auth.Api.Rest.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static void AddApiRestServices(this IServiceCollection services)
    {
        services.AddSingleton<IMiddlewareService, MiddlewareService>();
    }
}