﻿namespace AuthenticationService.Models.Users.DTO;

public class GetUserDTO
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
}

public record GetUserListDTO(string Id, string FirstName, string LastName, string UserName);