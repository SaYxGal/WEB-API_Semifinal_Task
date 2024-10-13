namespace TimetableService.Models.JWT;

public record JWTTokenValidationResult(bool IsValid, List<string> Roles);
