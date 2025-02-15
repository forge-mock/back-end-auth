using Auth.Api.Rest.Extensions;
using Auth.Api.Rest.Interfaces;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddCors(
    options =>
    {
        options.AddPolicy(
            "AllowDevelopment",
            policy =>
            {
                policy.WithOrigins("https://localhost:3000")
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithHeaders("X-Forge-Mock-Auth", "Content-Type");
            });
    });

builder.Services.AddApiRestServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(
        options =>
        {
            options.WithTheme(ScalarTheme.Purple).WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.Http);
        });
    app.Use(
        async (context, next) =>
        {
            var middlewareService = app.Services.GetRequiredService<IMiddlewareService>();
            middlewareService.ConfigureHeaders(ref context);
            await next();
        });
}
else
{
    app.Use(
        async (context, next) =>
        {
            if (!context.Request.Headers.ContainsKey("X-Forge-Mock-Auth") ||
                !context.Request.Headers.ContainsKey("Content-Type"))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var middlewareService = app.Services.GetRequiredService<IMiddlewareService>();
            middlewareService.ConfigureHeaders(ref context);
            await next();
        });
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
};

app.MapGet(
    "/weatherforecast",
    () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(
                index =>
                    new WeatherForecast(
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]))
            .ToArray();
        return forecast;
    }).WithName("GetWeatherForecast");

await app.RunAsync();

internal record WeatherForecast(DateOnly date, int temperatureC, string? summary)
{
    public int TemperatureF => 32 + (int)(this.temperatureC / 0.5556);
}