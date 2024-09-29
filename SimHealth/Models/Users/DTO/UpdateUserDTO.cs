namespace AuthenticationService.Models.Users.DTO;

public record UpdateUserDTO(string FirstName, string LastName, string Password);

public record UpdateUserAdminDTO(string FirstName, string LastName, string Password, string UserName, List<string> Roles) : UpdateUserDTO(FirstName, LastName, Password);
