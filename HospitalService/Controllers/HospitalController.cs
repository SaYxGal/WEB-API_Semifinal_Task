using HospitalService.Models;
using HospitalService.Models.Hospitals.DTO;
using HospitalService.Models.JWT;
using HospitalService.Services.Hospitals;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HospitalService.Controllers;
[Route("api/[controller]s")]
[ApiController]
public class HospitalController : ControllerBase
{
    private readonly IHospitalService _hospitalService;
    private readonly HttpClient _httpClient;
    private readonly string _validateTokenUrl = "/api/Authentication/Validate";

    public HospitalController(IHospitalService hospitalService, IHttpClientFactory httpClientFactory)
    {
        _hospitalService = hospitalService;
        _httpClient = httpClientFactory.CreateClient("Auth");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute][Required] int id, [FromHeader] string? authorization)
    {
        var validRequestResponse = await ValidateUser(authorization);

        if (validRequestResponse.IsValid)
        {
            return Ok(await _hospitalService.GetById(id));
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetHospitals([FromQuery][Required] int from, [FromQuery][Required] int count, [FromHeader] string? authorization)
    {
        var validRequestResponse = await ValidateUser(authorization);

        if (validRequestResponse.IsValid)
        {
            return Ok(await _hospitalService.GetHospitals(from, count));
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpGet("{id}/Rooms")]
    public async Task<IActionResult> GetHospitalRooms([FromRoute][Required] int id, [FromHeader] string? authorization)
    {
        var validRequestResponse = await ValidateUser(authorization);

        if (validRequestResponse.IsValid)
        {
            return Ok(await _hospitalService.GetHospitalRooms(id));
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody][Required] CreateHospitalDTO dto, [FromHeader] string? authorization)
    {
        var validRequestResponse = await ValidateUser(authorization);

        if (validRequestResponse.IsValid)
        {
            if (validRequestResponse.Roles.Where(i => i == UserRole.Admin).Any())
            {
                await _hospitalService.Create(dto);

                return Created();
            }
            else
            {
                return StatusCode(403);
            }
        }
        else
        {
            return Unauthorized();
        }

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute][Required] int id, [FromBody][Required] UpdateHospitalDTO dto, [FromHeader] string? authorization)
    {
        var validRequestResponse = await ValidateUser(authorization);

        if (validRequestResponse.IsValid)
        {
            if (validRequestResponse.Roles.Where(i => i == UserRole.Admin).Any())
            {
                await _hospitalService.Update(id, dto);

                return NoContent();
            }
            else
            {
                return StatusCode(403);
            }
        }
        else
        {
            return Unauthorized();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute][Required] int id, [FromHeader] string? authorization)
    {
        var validRequestResponse = await ValidateUser(authorization);

        if (validRequestResponse.IsValid)
        {
            if (validRequestResponse.Roles.Where(i => i == UserRole.Admin).Any())
            {
                await _hospitalService.Delete(id);

                return NoContent();
            }
            else
            {
                return StatusCode(403);
            }
        }
        else
        {
            return Unauthorized();
        }
    }

    [NonAction]
    private async Task<JWTTokenValidationResult> ValidateUser(string? authorization)
    {
        if (authorization == null)
        {
            return new JWTTokenValidationResult(false, []);
        }

        var queryParams = $"?accessToken={authorization}";

        var response = await _httpClient.GetAsync(_validateTokenUrl + queryParams);

        return JsonConvert.DeserializeObject<JWTTokenValidationResult>(await response.Content.ReadAsStringAsync()) ?? throw new Exception();
    }
}
