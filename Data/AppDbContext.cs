using Microsoft.EntityFrameworkCore;

namespace Pdae.Api.Data;

/// <summary>
/// Application database context. Entity sets are added as the domain model grows.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options);
