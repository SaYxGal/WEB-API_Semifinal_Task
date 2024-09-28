using AuthenticationService.Models.Users.DTO;
using AuthenticationService.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> SignIn([FromBody] SignInUserDTO dto)
    {
        try
        {
            return Ok(await _authentificationService.SignIn(dto));
        }
        catch
        {
            return Unauthorized();
        }
    }

    [HttpPost]
    public async Task<IActionResult> SignUp([FromBody] SignUpUserDTO dto)
    {
        try
        {
            await _authentificationService.SignUp(dto);

            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Validate(string accessToken)
    {
        return Ok(await _authentificationService.ValidateToken(accessToken));
    }

    [HttpPost]
    [Authorize]
    [Route("~/api/[controller]/SignOut")]
    public async Task<IActionResult> SignOutUser()
    {
        try
        {
            await _authentificationService.SignOut();

            return Ok();
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Refresh(UserTokenDTO dto)
    {
        try
        {
            return Ok(await _authentificationService.RefreshToken(dto));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
