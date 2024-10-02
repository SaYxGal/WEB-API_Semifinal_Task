using AuthenticationService.Services.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Controllers;
[Route("api/[controller]s")]
[ApiController]
[Authorize]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDoctor([FromRoute][Required] string id)
    {
        return Ok(await _doctorService.GetById(id));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? nameFilter, [FromQuery][Required] int from, [FromQuery][Required] int count)
    {
        return Ok(await _doctorService.GetDoctors(nameFilter, from, count));
    }
}
