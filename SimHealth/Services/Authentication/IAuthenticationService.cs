﻿using AuthenticationService.Models.JWT;
using AuthenticationService.Models.Users.DTO;

namespace AuthenticationService.Services.Authentication;

public interface IAuthenticationService
{
    public Task SignUp(SignUpUserDTO dto);

    public Task<UserTokenDTO> SignIn(SignInUserDTO dto);

    public Task SignOut();

    public Task<JWTTokenValidationResult> ValidateToken(string accessToken);

    public Task<UserTokenDTO> RefreshToken(string refreshToken);
}
