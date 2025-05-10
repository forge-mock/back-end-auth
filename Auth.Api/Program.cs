using Auth.Api.Rest.Extensions;
using Auth.Api.Rest.Interfaces;
using Auth.Persistence.Context;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Env.Load("../.env");

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
    options.AddPolicy(
        "Development",
        policy =>
        {
            policy.WithOrigins("https://localhost:3000")
                .AllowCredentials()
                .WithMethods("GET", "POST")
                .WithHeaders("X-XSRF-TOKEN", "Content-Type");
        }));
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
    options.Cookie.Name = "XSRF-TOKEN-COOKIE";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.AddDbContext<AuthContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("USER_IDENTITY_DB_CONNECTION_STRING")));
builder.Services.AddControllers();
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddApiRestServices();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options
        .WithTheme(ScalarTheme.Purple)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.Http));
}

app.Use(async (context, next) =>
{
    IMiddlewareService middlewareService = app.Services.GetRequiredService<IMiddlewareService>();
    middlewareService.ConfigureHeaders(ref context);
    await next();
});

app.UseCors("Development");
app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();