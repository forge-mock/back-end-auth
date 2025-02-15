using Auth.Api.Rest.Interfaces;
using Auth.Constants;

namespace Auth.Api.Rest.Services;

internal sealed class MiddlewareService : IMiddlewareService
{
    public void ConfigureHeaders(ref HttpContext context)
    {
        context.Response.Headers.Append(
            "Content-Security-Policy",
            $"{ContentSecurityPolicy.DefaultSrc} {ContentSecurityPolicy.ScriptSrc} {ContentSecurityPolicy.StyleSrc} {ContentSecurityPolicy.ImgSrc} {ContentSecurityPolicy.FontSrc} {ContentSecurityPolicy.ConnectSrc} {ContentSecurityPolicy.ObjectSrc}");
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        context.Response.Headers.Append("X-Forge-Mock-Auth", "Forge Mock Auth");
    }
}