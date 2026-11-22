using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pdae.Api.DTOs.Auth;
using Pdae.Api.DTOs.Users;
using Pdae.Api.Models;

namespace Pdae.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        var email = request.Email?.Trim();
        var displayName = request.DisplayName?.Trim();

        // Validate email
        if (string.IsNullOrWhiteSpace(email) ||
            !new EmailAddressAttribute().IsValid(email))
        {
            errors["email"] =
                ["A valid email address is required."];
        }

        // Validate display name
        if (string.IsNullOrWhiteSpace(displayName) ||
            displayName.Length < 2 ||
            displayName.Length > 100)
        {
            errors["displayName"] =
                ["Display name must be between 2 and 100 characters."];
        }

        // Validate password presence
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors["password"] = ["Password is required."];
        }

        
        if (errors.Count > 0)
        {
            foreach (var error in errors)
            {
                foreach (var message in error.Value)
                {
                    ModelState.AddModelError(error.Key, message);
                }
            }

            return ValidationProblem(ModelState);
        }

        // Check duplicate email
        var existingUser =
            await _userManager.FindByEmailAsync(email!);

        
        if (existingUser != null)
        {
            ModelState.AddModelError(
                "email",
                "Email is already registered.");

            return ValidationProblem(ModelState);
        }


        // Create user
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = displayName!,
            CharacterId = "hodlo",
            ExperienceLevel = "beginner",
            DailyGoalXp = 30,
            Timezone = "UTC",
            OnboardingCompleted = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var result = await _userManager.CreateAsync(
            user,
            request.Password!);

        if (!result.Succeeded)
        {
            var identityErrors = result.Errors
                .GroupBy(e =>
                    e.Code.StartsWith("Password", StringComparison.Ordinal)
                        ? "password"
                        : e.Code is "DuplicateEmail" or "DuplicateUserName"
                            ? "email"
                            : "registration")
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Description).ToArray());

            
            foreach (var error in identityErrors)
            {
                foreach (var message in error.Value)
                {
                    ModelState.AddModelError(error.Key, message);
                }
            }

            return ValidationProblem(ModelState);

        }

        // Return frontend-compatible profile
        var response = new UserProfileResponse(
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

        return Ok(response);
    }
}
