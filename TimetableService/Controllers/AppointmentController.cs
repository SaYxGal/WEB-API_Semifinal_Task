using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TimetableService.Services.Appointments;

namespace TimetableService.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> UnassignUserFromAppointment([FromRoute][Required] int id)
    {
        await _appointmentService.UnassignUser(id);

        return NoContent();
    }
}
