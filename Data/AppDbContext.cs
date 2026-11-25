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

        builder.Entity<Question>(entity =>
        {
            entity.HasKey(q => q.Id);

            entity.Property(q => q.Prompt)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(q => q.Explanation)
                .HasMaxLength(4000);

            entity.Property(q => q.ExerciseType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(q => q.ExerciseData)
                .HasColumnType("jsonb")
                .IsRequired();

            entity.Property(q => q.AnswerKey)
                .HasColumnType("jsonb");

            entity.HasOne(q => q.Lesson)
                .WithMany(l => l.Questions)
                .HasForeignKey(q => q.LessonId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(q => new
            {
                q.LessonId,
                q.Order
            }).IsUnique();

            entity.ToTable(table =>
                table.HasCheckConstraint(
                    "CK_Questions_ExerciseType",
                    """
                    "ExerciseType" IN (
                        'multiple_choice',
                        'true_false',
                        'fill_blank',
                        'match_pairs',
                        'order_steps',
                        'read_chart',
                        'spot_scam',
                        'risk_calculator',
                        'dca_timeline',
                        'story_card'
                    )
                    """
                )
            );
        });

        builder.Entity<AnswerOption>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Text)
                .IsRequired()
                .HasMaxLength(2000);

            entity.HasOne(a => a.Question)
                .WithMany(q => q.AnswerOptions)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(a => new
            {
                a.QuestionId,
                a.Order
            }).IsUnique();
        });
    }

    public DbSet<Question> Questions
        => Set<Question>();

    public DbSet<AnswerOption> AnswerOptions
        => Set<AnswerOption>();
}