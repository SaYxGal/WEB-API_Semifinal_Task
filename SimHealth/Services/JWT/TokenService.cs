using AuthenticationService.Confuguration;
using AuthenticationService.Models.JWT;
using AuthenticationService.Models.Users;
using AuthenticationService.Models.Users.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthenticationService.Services.JWT;

public class TokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<User> _userManager;

    public TokenService(JwtSettings jwtSettings, UserManager<User> userManager)
    {
        _jwtSettings = jwtSettings;
        _userManager = userManager;
    }

    public async Task<UserTokenDTO> GenerateToken(User user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!)
        };

        foreach (var userRole in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = GenerateAccessToken(claims);

        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_jwtSettings.RefreshTokenValidityInDays);

        await _userManager.UpdateAsync(user);
        await _userManager.AddClaimsAsync(user, claims);

        var response = new UserTokenDTO
        (
            new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken
        );

        return response;
    }

    public async Task<JWTTokenValidationResult> ValidateToken(string accessToken)
    {
        var validationParameters = GetValidationParameters();

        var result = await new JwtSecurityTokenHandler().ValidateTokenAsync(accessToken, validationParameters);

        return new JWTTokenValidationResult(
            result.IsValid ? result.ClaimsIdentity.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value : null,
            result.IsValid,
            result.IsValid ? result.ClaimsIdentity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList() : []);
    }

    public async Task<UserTokenDTO> RefreshToken(string refreshToken)
    {
        var user = await
            _userManager.Users
                .Where(i => i.RefreshToken == refreshToken)
                .FirstOrDefaultAsync()
                ?? throw new SecurityTokenException("Invalid refresh token");

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        var newAccessToken = GenerateAccessToken(await _userManager.GetClaimsAsync(user));
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new UserTokenDTO
        (
            new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            newRefreshToken
        );
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private JwtSecurityToken GenerateAccessToken(IList<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        return new JwtSecurityToken(
           _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtSettings.DurationInMinutes)),
            signingCredentials: credentials
        );
    }

    private TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateLifetime = true
        };
    }
}
