using AutoMapper;
using TimetableService.Models.Appointments.DTO;

namespace TimetableService.Models.Appointments;

public class AppointmentAutomapperProfiles : Profile
{
    public AppointmentAutomapperProfiles()
    {
        CreateMap<Appointment, GetAppointmentDTO>();
    }
}
