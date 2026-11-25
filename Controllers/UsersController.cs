using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pdae.Api.DTOs.Users;
using Pdae.Api.Models;

namespace Pdae.Api.Controllers;

[ApiController]
[Authorize]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(CreateProfileResponse(user));
    }

    [HttpPatch("me")]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateUserProfileRequest request)
    {
        if (!HasExpectedRequestHeader())
        {
            return Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Forbidden",
                detail: "Missing required request header.");
        }

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Unauthorized();
        }

        // Validate display name
        if (request.DisplayName is not null)
        {
            var displayName = request.DisplayName.Trim();

            if (displayName.Length < 2 ||
                displayName.Length > 100)
            {
                ModelState.AddModelError(
                    "displayName",
                    "Display name must be between 2 and 100 characters.");
            }
        }

        // Validate timezone
        if (request.Timezone is not null)
        {
            var timezone = request.Timezone.Trim();

            if (!TimeZoneInfo.TryFindSystemTimeZoneById(
                timezone, out _))
            {
                ModelState.AddModelError(
                    "timezone",
                    "Invalid timezone.");
            }
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        // Update profile fields
        if (request.DisplayName is not null)
        {
            user.DisplayName = request.DisplayName.Trim();
        }

        if (request.Timezone is not null)
        {
            user.Timezone = request.Timezone.Trim();
        }

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            if (result.Errors.Any(
                e => e.Code == "ConcurrencyFailure"))
            {
                return Conflict(new
                {
                    message = "Profile was modified. Please retry."
                });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    "profile", error.Description);
            }

            return ValidationProblem(ModelState);
        }

        return Ok(CreateProfileResponse(user));
    }

    private static UserProfileResponse CreateProfileResponse(
        ApplicationUser user)
    {
        return new UserProfileResponse(
            Id: user.Id,
            Email: user.Email!,
            DisplayName: user.DisplayName,
            CharacterId: user.CharacterId,
            ExperienceLevel: user.ExperienceLevel,
            Interests: [],
            DailyGoalXp: user.DailyGoalXp,
            Xp: 0,
            Gems: 0,
            Hearts: 5,
            MaxHearts: 5,
            Timezone: user.Timezone,
            Streak: new StreakResponse(
                Current: 0,
                Longest: 0,
                LastLessonDate: null,
                FreezesEquipped: 0,
                RestDay: null
            ),
            OnboardingCompleted: user.OnboardingCompleted,
            CreatedAt: user.CreatedAt
        );
    }

    private bool HasExpectedRequestHeader()
    {
        return Request.Headers.TryGetValue(
                   "X-Hodlo-Request",
                   out var value)
               && value == "1";
    }
}