using System.ComponentModel.DataAnnotations;

namespace Pdae.Api.DTOs.Users;

public record UpdateUserProfileRequest(
    [StringLength(100, MinimumLength = 2)]
    string? DisplayName,

    [StringLength(100)]
    string? Timezone
);