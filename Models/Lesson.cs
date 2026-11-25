namespace Pdae.Api.Models;

public class Lesson
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid LearningModuleId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public int OrderIndex { get; set; }

    public int XpReward { get; set; } = 10;

    public DateTimeOffset CreatedAt { get; set; }
        = DateTimeOffset.UtcNow;

    public LearningModule LearningModule { get; set; }
        = null!;

    public ICollection<Question> Questions { get; set; }
        = new List<Question>();
}