using Auth.Api.Rest.Constants;
using Auth.Api.Rest.Extensions;
using Auth.Api.Rest.Interfaces;
using Auth.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

builder.Services.AddCors(
    options =>
        options.AddPolicy(
            "AllowDevelopment",
            policy =>
            {
                policy.WithOrigins("https://localhost:3000")
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithHeaders(HttpHeaders.ForgeMockAuth, HttpHeaders.ForgeMockAuth);
            }));

builder.Services.AddDbContext<AuthContext>(
    options =>
        options.UseNpgsql(Environment.GetEnvironmentVariable("AUTH_DB_CONNECTION_STRING")));
builder.Services.AddControllers();
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddApiRestServices();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(
        options =>
            options.WithTheme(ScalarTheme.Purple).WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.Http));
    app.Use(
        async (context, next) =>
        {
            IMiddlewareService middlewareService = app.Services.GetRequiredService<IMiddlewareService>();
            middlewareService.ConfigureHeaders(ref context);
            await next();
        });
}
else
{
    app.Use(
        async (context, next) =>
        {
            if (!context.Request.Headers.ContainsKey(HttpHeaders.ForgeMockAuth) ||
                !context.Request.Headers.ContainsKey(HttpHeaders.ContentType))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            IMiddlewareService middlewareService = app.Services.GetRequiredService<IMiddlewareService>();
            middlewareService.ConfigureHeaders(ref context);
            await next();
        });
}

app.UseCors("AllowDevelopment");
app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();