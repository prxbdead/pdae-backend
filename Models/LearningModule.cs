namespace Pdae.Api.Models;

public class LearningModule
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int OrderIndex { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
        = DateTimeOffset.UtcNow;

    public ICollection<Lesson> Lessons { get; set; }
        = new List<Lesson>();
}