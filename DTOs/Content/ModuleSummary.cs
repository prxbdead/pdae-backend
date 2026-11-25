namespace Pdae.Api.DTOs.Content;

public record ModuleSummary(
    Guid Id,
    string Title,
    string Description,
    int OrderIndex,
    int LessonCount
);