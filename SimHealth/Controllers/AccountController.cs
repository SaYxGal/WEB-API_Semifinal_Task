using AuthenticationService.Models.Users;
using AuthenticationService.Models.Users.DTO;
using AuthenticationService.Services.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("Me")]
    public async Task<IActionResult> Me()
    {
        return Ok(await _accountService.Get(User));
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] UpdateUserDTO dto)
    {
        var user = await _accountService.Get(User);

        await _accountService.Update(user.Id, dto);

        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = UserRole.Admin)]
    public async Task<IActionResult> GetAll(int from, int count)
    {
        return Ok(await _accountService.GetAll(from, count));
    }

    [HttpPost]
    [Authorize(Roles = UserRole.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateUserDTO dto)
    {
        await _accountService.Create(dto);

        return Created();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = UserRole.Admin)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserAdminDTO dto)
    {
        await _accountService.Update(id, dto);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = UserRole.Admin)]
    public async Task<IActionResult> Delete(string id)
    {
        await _accountService.Delete(id);

        return NoContent();
    }
}
