namespace Pdae.Api.DTOs.Content;

public record LessonSummary(
    Guid Id,
    Guid LearningModuleId,
    string Title,
    string Description,
    int OrderIndex,
    int XpReward
);