using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using TimetableService.Attributes;
using TimetableService.Models;
using TimetableService.Models.Timetables.DTO;
using TimetableService.Services.Appointments;
using TimetableService.Services.Timetables;

namespace TimetableService.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TimetableController : ControllerBase
{
    private readonly ITimetableService _timetableService;
    private readonly IAppointmentService _appointmentService;

    public TimetableController(ITimetableService timetableService, IAppointmentService appointmentService)
    {
        _timetableService = timetableService;
        _appointmentService = appointmentService;
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

    [HttpGet("/Hospital/{hospitalId}")]
    [Authorize]
    public async Task<IActionResult> GetHospitalRecords([FromRoute][Required] int hospitalId, [FromQuery][Required] string from, [FromQuery][Required] string to)
    {
        return Ok(await _timetableService.GetHospitalTimetable(hospitalId, from, to));
    }

    [HttpGet("/Doctor/{doctorId}")]
    [Authorize]
    public async Task<IActionResult> GetDoctorRecords([FromRoute][Required] string doctorId, [FromQuery][Required] string from, [FromQuery][Required] string to)
    {
        return Ok(await _timetableService.GetDoctorTimetable(doctorId, from, to));
    }

    [HttpGet("/Hospital/{hospitalId}/Room/{room}")]
    [Authorize(UserRole.Admin, UserRole.Manager, UserRole.Doctor)]
    public async Task<IActionResult> GetHospitalRecords([FromRoute][Required] int hospitalId, [FromRoute][Required] string room, [FromQuery][Required] string from, [FromQuery][Required] string to)
    {
        return Ok(await _timetableService.GetHospitalRoomTimetable(hospitalId, room, from, to));
    }

    [HttpGet("{id}/Appointments")]
    [Authorize]
    public async Task<IActionResult> GetFreeAppointments([FromRoute][Required] int id)
    {
        return Ok(await _appointmentService.GetFreeAppointments(id));
    }

    [HttpPost("{id}/Appointments")]
    [Authorize]
    public async Task<IActionResult> AssignUserToAppointment([FromRoute][Required] int id, [FromQuery][Required] DateTime time)
    {
        await _appointmentService.AssignUserToAppointment(id, time);

        return Created();
    }
}
