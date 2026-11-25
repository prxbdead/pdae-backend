namespace Pdae.Api.DTOs.Auth;

public record LoginRequest(
    string? Email,
    string? Password
);
