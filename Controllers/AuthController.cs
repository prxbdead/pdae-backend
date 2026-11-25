using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pdae.Api.DTOs.Auth;
using Pdae.Api.DTOs.Users;
using Pdae.Api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Pdae.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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
    
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        
        if (!HasExpectedRequestHeader())
        {
            return Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Forbidden",
                detail: "Missing required request header.");
        }

        
        var email = request.Email?.Trim();

        // 1. Validate required fields
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            ModelState.AddModelError(
                "credentials",
                "Email and password are required.");

            return ValidationProblem(ModelState);
        }

        // 2. Find user by email
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid credentials",
                detail: "Invalid email or password.");
        }

        // 3. Verify password and create auth cookie
        var result = await _signInManager.PasswordSignInAsync(
            user,
            request.Password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid credentials",
                detail: "Invalid email or password.");
        }

        // 4. Return successful login response
        return Ok(new
        {
            message = "Login successful"
        });
    }
    
    
    
    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!HasExpectedRequestHeader())
        {
            return Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Forbidden",
                detail: "Missing required request header.");
        }

        await _signInManager.SignOutAsync();

        return NoContent();
    }

    
    
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(new
        {
            user.Id,
            user.Email,
            user.DisplayName,
            user.CharacterId,
            user.OnboardingCompleted
        });
    }
    
    
    private bool HasExpectedRequestHeader()
    {
        return Request.Headers.TryGetValue(
                   "X-Hodlo-Request",
                   out var value)
               && value == "1";
    }
}
