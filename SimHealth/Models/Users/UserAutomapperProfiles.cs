using AuthenticationService.Models.Users.DTO;
using AutoMapper;

namespace AuthenticationService.Models.Users;

public class UserAutomapperProfiles : Profile
{
    public UserAutomapperProfiles()
    {
        CreateMap<SignUpUserDTO, User>();
    }
}
