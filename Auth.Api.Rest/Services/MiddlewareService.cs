using Auth.Api.Rest.Interfaces;

namespace Auth.Api.Rest.Services;

internal sealed class MiddlewareService(IWebHostEnvironment env) : IMiddlewareService
{
    private readonly string[] csp =
    [
        "default-src 'self'",
        $"script-src 'self' {(env.IsDevelopment() ? "'unsafe-eval' 'unsafe-inline'" : string.Empty)}",
        $"style-src 'self' {(env.IsDevelopment() ? "'unsafe-inline'" : string.Empty)}",
        "img-src 'self'",
        "font-src 'self' https://fonts.scalar.com/",
        "connect-src 'self'",
        "frame-ancestors 'none'",
        "form-action 'self'",
        "base-uri 'self'",
        "object-src 'none'",
    ];

    public void ConfigureHeaders(ref HttpContext context)
    {
        context.Response.Headers.Append("Content-Security-Policy", string.Join("; ", csp));
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
        context.Response.Headers.Append("Referrer-Policy", "no-referrer");
        context.Response.Headers.Append(
            "Permissions-Policy",
            "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
        context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "require-corp");
        context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin");
        context.Response.Headers.Append("Cross-Origin-Resource-Policy", "same-origin");
    }
}