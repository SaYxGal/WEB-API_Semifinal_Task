namespace AuthenticationService.Models.Users.DTO;

public record UserTokenDTO(string AccessToken, string RefreshToken);