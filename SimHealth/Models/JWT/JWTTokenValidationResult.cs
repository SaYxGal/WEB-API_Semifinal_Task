namespace AuthenticationService.Models.JWT;

public record JWTTokenValidationResult(bool IsValid, List<string> Roles);
