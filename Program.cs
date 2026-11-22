
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pdae.Api.Data;
using Pdae.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI configuration
builder.Services.AddOpenApi();

// PostgreSQL database configuration
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ASP.NET Core Identity configuration
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Authentication cookie configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "Hodlo.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

// Authorization services
builder.Services.AddAuthorization();

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
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("DevelopmentCors");
}
else
{
    app.UseHttpsRedirection();
}

// Authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Health endpoint
app.MapGet("/health", () =>
    new HealthResponse("ok", DateTimeOffset.UtcNow))
   .WithName("GetHealth");

app.Run();

record HealthResponse(string Status, DateTimeOffset Timestamp);
