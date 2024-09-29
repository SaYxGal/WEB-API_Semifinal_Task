using AuthenticationService.Models.Users.DTO;
using AutoMapper;

namespace AuthenticationService.Models.Users;

public class UserAutomapperProfiles : Profile
{
    public UserAutomapperProfiles()
    {
        CreateMap<SignUpUserDTO, User>();
        CreateMap<User, GetUserDTO>()
            .ForMember(x => x.Roles, opt => opt.Ignore());
        CreateMap<User, GetUserListDTO>();
        CreateMap<CreateUserDTO, User>();
        CreateMap<UpdateUserDTO, User>();
        CreateMap<UpdateUserAdminDTO, User>();
    }
}
