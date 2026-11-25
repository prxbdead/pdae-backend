using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pdae.Api.Models;

namespace Pdae.Api.Data;

public class AppDbContext(
    DbContextOptions<AppDbContext> options
) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<LearningModule> LearningModules
        => Set<LearningModule>();

    public DbSet<Lesson> Lessons
        => Set<Lesson>();

    protected override void OnModelCreating(
        ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<LearningModule>(entity =>
        {
            entity.HasKey(m => m.Id);

            entity.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(m => m.Description)
                .HasMaxLength(1000);

            entity.HasIndex(m => m.OrderIndex);
        });

        builder.Entity<Lesson>(entity =>
        {
            entity.HasKey(l => l.Id);

            entity.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(l => l.Description)
                .HasMaxLength(1000);

            entity.Property(l => l.Content)
                .IsRequired();

            entity.HasOne(l => l.LearningModule)
                .WithMany(m => m.Lessons)
                .HasForeignKey(l => l.LearningModuleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(l => new
            {
                l.LearningModuleId,
                l.OrderIndex
            }).IsUnique();
        });
    }
}