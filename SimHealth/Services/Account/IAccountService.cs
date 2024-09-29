using AuthenticationService.Models.Users.DTO;
using System.Security.Claims;

namespace AuthenticationService.Services.Account;

public interface IAccountService
{
    public Task<GetUserDTO> Get(ClaimsPrincipal principal);

    public Task Update(string id, UpdateUserDTO dto);

    public Task<List<GetUserListDTO>> GetAll(int from, int count);

    public Task Create(CreateUserDTO dto);

    public Task Delete(string id);
}
