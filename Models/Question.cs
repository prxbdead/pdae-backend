namespace Pdae.Api.Models;

public class Question
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid LessonId { get; set; }

    public string Prompt { get; set; } = string.Empty;

    public string? Explanation { get; set; }

    public string ExerciseType { get; set; }
        = ExerciseTypes.MultipleChoice;

    public int Order { get; set; }

    public string ExerciseData { get; set; } = "{}";

    public string? AnswerKey { get; set; }

    public Lesson Lesson { get; set; } = null!;

    public ICollection<AnswerOption> AnswerOptions { get; set; }
        = new List<AnswerOption>();
}