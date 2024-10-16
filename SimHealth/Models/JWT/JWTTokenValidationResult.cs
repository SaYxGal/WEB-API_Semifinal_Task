namespace AuthenticationService.Models.JWT;

public record JWTTokenValidationResult(string? userId, bool IsValid, List<string> Roles);
