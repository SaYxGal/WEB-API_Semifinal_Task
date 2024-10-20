using AuthenticationService.Models.Users.DTO;
using AuthenticationService.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authentificationService;

    public AuthenticationController(IAuthenticationService authentificationService)
    {
        _authentificationService = authentificationService;
    }

    [HttpPost]
    public async Task<IActionResult> SignIn([FromBody][Required] SignInUserDTO dto)
    {
        return Ok(await _authentificationService.SignIn(dto));
    }

    [HttpPost]
    public async Task<IActionResult> SignUp([FromBody][Required] SignUpUserDTO dto)
    {
        await _authentificationService.SignUp(dto);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Validate([FromQuery][Required] string accessToken)
    {
        return Ok(await _authentificationService.ValidateToken(accessToken));
    }

    [HttpPost]
    [Authorize]
    [Route("~/api/[controller]/SignOut")]
    public async Task<IActionResult> SignOutUser()
    {
        await _authentificationService.SignOut();

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Refresh([FromQuery][Required] string refreshToken)
    {
        return Ok(await _authentificationService.RefreshToken(refreshToken));
    }
}
