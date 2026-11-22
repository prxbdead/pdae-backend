using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pdae.Api.Models;

namespace Pdae.Api.Data;

public class AppDbContext(
    DbContextOptions<AppDbContext> options
) : IdentityDbContext<ApplicationUser>(options);
