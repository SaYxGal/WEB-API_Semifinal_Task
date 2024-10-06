using AuthenticationService.Models.JWT;
using AuthenticationService.Models.Users;
using AuthenticationService.Models.Users.DTO;
using AuthenticationService.Services.JWT;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Services.Authentication;

public class AuthenticationServiceImpl : IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly TokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthenticationServiceImpl(UserManager<User> userManager, IMapper mapper, TokenService tokenService, SignInManager<User> signInManager)
    {
        _mapper = mapper;
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
    }

    public async Task<UserTokenDTO> RefreshToken(UserTokenDTO tokens)
    {
        return await _tokenService.RefreshToken(tokens);
    }

    public async Task<UserTokenDTO> SignIn(SignInUserDTO dto)
    {
        var user = await _userManager.FindByNameAsync(dto.UserName);

        if (user != null && !user.IsDeleted && await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            return await _tokenService.GenerateToken(user);
        }
        else
        {
            throw new UnauthorizedAccessException("Логин или пароль пользователя указаны некорректно");
        }
    }

    public async Task SignOut()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task SignUp(SignUpUserDTO dto)
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

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            throw new ApplicationException(result.Errors
                .Select(x => string.Format("{0}:{1}", x.Code, x.Description))
                .Aggregate((current, next) => current + ", " + next));

        await _userManager.AddToRoleAsync(user, UserRole.User);
    }

    public async Task<JWTTokenValidationResult> ValidateToken(string accessToken)
    {
        return await _tokenService.ValidateToken(accessToken);
    }
}
