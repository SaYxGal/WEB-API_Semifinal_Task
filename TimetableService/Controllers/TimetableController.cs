using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using TimetableService.Configuration;
using TimetableService.Models;
using TimetableService.Models.JWT;
using TimetableService.Models.Timetables.DTO;
using TimetableService.Services.Timetables;

namespace TimetableService.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TimetableController : ControllerBase
{
    private readonly ITimetableService _timetableService;
    private readonly HttpClient _httpClient;
    private readonly BaseAddresses _baseAddresses;
    private readonly string _validateTokenUrl = "api/Authentication/Validate";
    private readonly string _doctorGetUrl = "api/Doctors/";
    private readonly string _hospitalGetUrl = "api/Hospitals/";

    public TimetableController(ITimetableService timetableService, IHttpClientFactory httpClientFactory, BaseAddresses baseAddresses)
    {
        _timetableService = timetableService;
        _httpClient = httpClientFactory.CreateClient();
        _baseAddresses = baseAddresses;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody][Required] AddTimetableRecordDTO dto, [FromHeader] string? authorization)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var validRequestResponse = await ValidateUser(authorization);

        if (validRequestResponse.IsValid)
        {
            if (validRequestResponse.Roles.Where(i => i == UserRole.Admin || i == UserRole.Manager).Any())
            {
                await ValidateTimetableValues(dto.DoctorId, dto.HospitalId, dto.Room, authorization);

                if (ModelState.IsValid)
                {
                    await _timetableService.AddRecordToTimetable(dto);

                    return Created();
                }

                return BadRequest(ModelState);
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
    public async Task<IActionResult> Update([FromRoute][Required] int id, [FromBody][Required] UpdateTimetableRecordDTO dto, [FromHeader] string? authorization)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var validRequestResponse = await ValidateUser(authorization);

        if (validRequestResponse.IsValid)
        {
            if (validRequestResponse.Roles.Where(i => i == UserRole.Admin || i == UserRole.Manager).Any())
            {
                await ValidateTimetableValues(dto.DoctorId, dto.HospitalId, dto.Room, authorization);

                if (ModelState.IsValid)
                {
                    await _timetableService.UpdateRecordFromTimetable(id, dto);

                    return NoContent();
                }

                return BadRequest(ModelState);
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

        var validRequest = new HttpRequestMessage()
        {
            RequestUri = new Uri(_baseAddresses.AuthService + _validateTokenUrl + queryParams),
            Method = HttpMethod.Get
        };

        var response = await _httpClient.SendAsync(validRequest);

        return JsonConvert.DeserializeObject<JWTTokenValidationResult>(await response.Content.ReadAsStringAsync()) ?? throw new Exception();
    }

    [NonAction]
    private async Task ValidateTimetableValues(string doctorId, int hospitalId, string room, string? authorization)
    {
        var doctorRequest = new HttpRequestMessage()
        {
            RequestUri = new Uri(_baseAddresses.AuthService + _doctorGetUrl + doctorId),
            Method = HttpMethod.Get,
        };

        doctorRequest.Headers.TryAddWithoutValidation("Authorization", authorization);

        var isDoctorExistsResponse = await _httpClient.SendAsync(doctorRequest);

        var hospitalRequest = new HttpRequestMessage()
        {
            RequestUri = new Uri(_baseAddresses.HospitalService + _hospitalGetUrl + hospitalId + "/Rooms"),
            Method = HttpMethod.Get,
        };

        hospitalRequest.Headers.TryAddWithoutValidation("Authorization", authorization);

        var hospitalRoomsResponse = await _httpClient.SendAsync(hospitalRequest);


        if (!isDoctorExistsResponse.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", $"Доктор с id {doctorId} не найден");
        }

        if (!hospitalRoomsResponse.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", $"Больница с id {hospitalId} не найдена");
        }
        else
        {
            var rooms = JsonConvert.DeserializeObject<List<string>>(await hospitalRoomsResponse.Content.ReadAsStringAsync());

            if (rooms == null || !rooms.Where(i => i == room).Any())
            {
                ModelState.AddModelError("", $"Кабинет с названием {room} не найден");
            }
        }
    }
}
