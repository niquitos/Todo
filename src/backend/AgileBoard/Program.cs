using System.Runtime.CompilerServices;
using AgileBoard.Adapters.Persistence;
using AgileBoard.Adapters.WebApi.Filters;
using AgileBoard.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[assembly: InternalsVisibleTo("AgileBoard.Tests")]

var builder = WebApplication.CreateBuilder(args);

// Allow Npgsql to treat Unspecified DateTime as UTC (required for JSON-deserialized dates)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<ExceptionHandlingMiddleware>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Ensure database and seed default data on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();

    try
    {
        if (!dbContext.Sprints.Any(s => s.IsDefault))
        {
            var defaultSprint = Sprint.Create(
                "Не запланировано",
                DateTime.MinValue,
                DateTime.MaxValue,
                "Задачи без привязки к спринту",
                isDefault: true);
            dbContext.Sprints.Add(defaultSprint);
            dbContext.SaveChanges();
        }
    }
    catch
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var defaultSprint = Sprint.Create(
            "Не запланировано",
            DateTime.MinValue,
            DateTime.MaxValue,
            "Задачи без привязки к спринту",
            isDefault: true);
        dbContext.Sprints.Add(defaultSprint);
        dbContext.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseExceptionHandler("/error");

app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
