using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(
        options =>
        {
            options.WithTheme(ScalarTheme.Purple).WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.Http);
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