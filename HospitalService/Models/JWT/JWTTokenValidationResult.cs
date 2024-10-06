namespace HospitalService.Models.JWT;

public record JWTTokenValidationResult(bool IsValid, List<string> Roles);
