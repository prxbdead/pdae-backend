namespace Pdae.Api.Models;

public class AnswerOption
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid QuestionId { get; set; }

    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    public int Order { get; set; }

    public Question Question { get; set; } = null!;
}