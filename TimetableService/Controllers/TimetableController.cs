using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using TimetableService.Attributes;
using TimetableService.Models;
using TimetableService.Models.Timetables.DTO;
using TimetableService.Services.Timetables;

namespace TimetableService.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TimetableController : ControllerBase
{
    private readonly ITimetableService _timetableService;

    public TimetableController(ITimetableService timetableService)
    {
        _timetableService = timetableService;
    }

    [HttpPost]
    [Authorize(UserRole.Admin, UserRole.Manager)]
    public async Task<IActionResult> Create([FromBody][Required] AddTimetableRecordDTO dto)
    {
        await _timetableService.AddRecordToTimetable(dto);

        return Created();
    }

    [HttpPut("{id}")]
    [Authorize(UserRole.Admin, UserRole.Manager)]
    public async Task<IActionResult> Update([FromRoute][Required] int id, [FromBody][Required] UpdateTimetableRecordDTO dto)
    {
        await _timetableService.UpdateRecordFromTimetable(id, dto);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(UserRole.Admin, UserRole.Manager)]
    public async Task<IActionResult> Delete([FromRoute][Required] int id)
    {
        await _timetableService.DeleteRecordFromTimetable(id);

        return NoContent();
    }

    [HttpDelete("{doctorId}")]
    [Authorize(UserRole.Admin, UserRole.Manager)]
    public async Task<IActionResult> DeleteDoctorRecords([FromRoute][Required] string doctorId)
    {
        await _timetableService.DeleteDoctorRecordsFromTimetable(doctorId);

        return NoContent();
    }

    [HttpDelete("{hospitalId}")]
    [Authorize(UserRole.Admin, UserRole.Manager)]
    public async Task<IActionResult> DeleteHospitalRecords([FromRoute][Required] int hospitalId)
    {
        await _timetableService.DeleteHospitalRecordsFromTimetable(hospitalId);

        return NoContent();
    }

    /*[NonAction]
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
    }*/
}
