using AuthenticationService.Data;
using AuthenticationService.Models.Users;
using AuthenticationService.Models.Users.DTO;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthenticationService.Services.Account;

public class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public AccountService(UserManager<User> userManager, IMapper mapper, DataContext dataContext)
    {
        _userManager = userManager;
        _mapper = mapper;
        _dataContext = dataContext;
    }

    public async Task Create(CreateUserDTO dto)
    {
        var userExists = await _userManager.FindByNameAsync(dto.UserName);
        if (userExists != null)
        {
            throw new ApplicationException("Пользователь с таким логином уже существует");
        }

        var user = new User()
        {
            SecurityStamp = Guid.NewGuid().ToString()
        };

        _mapper.Map(dto, user);

        ProcessErrors(await _userManager.CreateAsync(user, dto.Password));

        ProcessErrors(await _userManager.AddToRolesAsync(user, dto.Roles));
    }

    public async Task<GetUserDTO> Get(ClaimsPrincipal principal)
    {
        var user = await _userManager.GetUserAsync(principal);

        if (user == null)
        {
            throw new ApplicationException("Пользователь не найден");
        }

        var result = _mapper.Map<User, GetUserDTO>(user);

        result.Roles =
            principal.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

        return result;
    }

    public async Task<List<GetUserListDTO>> GetAll(int from, int count)
    {
        return await
            _dataContext.Users
                .Where(x => !x.IsDeleted)
                .Skip(from - 1).Take(count)
                .ProjectTo<GetUserListDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
    }

    public async Task Update(string id, UpdateUserDTO dto)
    {
        var user = await Find(id);

        if (dto is UpdateUserAdminDTO dtoAdmin)
        {
            _mapper.Map(dtoAdmin, user);

            var currentRoles = await _userManager.GetRolesAsync(user);

            ProcessErrors(await _userManager.RemoveFromRolesAsync(user, currentRoles));

            ProcessErrors(await _userManager.AddToRolesAsync(user, dtoAdmin.Roles));
        }
        else
        {
            _mapper.Map(dto, user);
        }

        if (!await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            ProcessErrors(await _userManager.ResetPasswordAsync(user, token, dto.Password));
        }

        ProcessErrors(await _userManager.UpdateAsync(user));
    }

    public async Task Delete(string id)
    {
        var user = await Find(id);

        user.IsDeleted = true;

        ProcessErrors(await _userManager.UpdateAsync(user));
    }

    private async Task<User> Find(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null || user.IsDeleted)
        {
            throw new ApplicationException("Пользователь не найден");
        }

        return user;
    }

    private static void ProcessErrors(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new ApplicationException(result.Errors
                .Select(x => string.Format("{0}:{1}", x.Code, x.Description))
                .Aggregate((current, next) => current + ", " + next));
    }
}
