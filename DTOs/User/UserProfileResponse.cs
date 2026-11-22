namespace Pdae.Api.DTOs.Users;

public record StreakResponse(
    int Current,
    int Longest,
    string? LastLessonDate,
    int FreezesEquipped,
    int? RestDay
);

public record UserProfileResponse(
    string Id,
    string Email,
    string DisplayName,
    string CharacterId,
    string ExperienceLevel,
    string[] Interests,
    int DailyGoalXp,
    int Xp,
    int Gems,
    int Hearts,
    int MaxHearts,
    string Timezone,
    StreakResponse Streak,
    bool OnboardingCompleted,
    DateTimeOffset CreatedAt
);