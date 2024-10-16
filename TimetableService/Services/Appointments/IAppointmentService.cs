using TimetableService.Models.Appointments.DTO;

namespace TimetableService.Services.Appointments;

public interface IAppointmentService
{
    public Task<List<GetAppointmentDTO>> GetFreeAppointments(int timetableId);
}
