using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Models.Users;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? RefreshToken { get; set; }

    public DateTime RefreshTokenExpiryTime { get; set; }

    public bool IsDeleted { get; set; } = false;
}
