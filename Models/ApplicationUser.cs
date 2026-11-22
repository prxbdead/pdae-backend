using Microsoft.AspNetCore.Identity;

namespace Pdae.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;

    public string CharacterId { get; set; } = "hodlo";

    public string ExperienceLevel { get; set; } = "beginner";

    public int DailyGoalXp { get; set; } = 30;

    public string Timezone { get; set; } = "UTC";

    public bool OnboardingCompleted { get; set; } = false;

    public DateTimeOffset CreatedAt { get; set; }
        = DateTimeOffset.UtcNow;
}
