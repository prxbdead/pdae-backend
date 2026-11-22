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

// CORS konfiguráció
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
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

app.UseCors();

// Health endpoint
app.MapGet("/health", () =>
        new HealthResponse("ok", DateTimeOffset.UtcNow))
    .WithName("GetHealth");

app.Run();

record HealthResponse(string Status, DateTimeOffset Timestamp);