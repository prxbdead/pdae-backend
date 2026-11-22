using Microsoft.EntityFrameworkCore;
using Pdae.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI konfiguráció
builder.Services.AddOpenApi();

// PostgreSQL adatbázis kapcsolat
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException(
                           "Connection string 'DefaultConnection' is not configured. " +
                           "Set it in appsettings.Development.json or the ConnectionStrings__DefaultConnection environment variable.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// CORS configuration for local development
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCors", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// OpenAPI csak fejlesztői környezetben
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
}

if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentCors");
}

// Health endpoint
app.MapGet("/health", () =>
        new HealthResponse("ok", DateTimeOffset.UtcNow))
    .WithName("GetHealth");

app.Run();

record HealthResponse(string Status, DateTimeOffset Timestamp);