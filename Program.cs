using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Context>(db => db.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
// Add services to the container.

builder.Services.AddApplicationInsightsTelemetry();
var app = builder.Build();

// Configure the HTTP request pipeline.

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapGet("/bob", async (Context ctx) =>
{
    var user = await ctx.Users.OrderBy(u => u.Id).LastOrDefaultAsync();
    await ctx.Users.AddAsync(new User
    {
        Age = Random.Shared.Next(0, 80),
        Name = "Bob"
    });
    await ctx.SaveChangesAsync();
    return Results.Ok(user);
});

app.MapGet("/throw", () => {
    throw new Exception("Wazzup!");
});

try
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider
        .GetRequiredService<Context>()
        .Database.MigrateAsync();
}
catch (Exception e)
{
    app.Logger.LogError("Could not run migration! Reason: {Reason}", e.Message);
}


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}