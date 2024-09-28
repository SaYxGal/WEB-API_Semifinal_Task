namespace AuthenticationService.Models.Users.DTO;

public record CreateUserDTO(string FirstName, string LastName, string UserName, string Password, List<string> Roles);
