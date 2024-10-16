namespace HospitalService.Models.JWT;

public record JWTTokenValidationResult(string? UserId, bool IsValid, List<string> Roles);
