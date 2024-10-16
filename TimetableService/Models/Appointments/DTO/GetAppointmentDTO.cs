namespace TimetableService.Models.Appointments.DTO;

public record GetAppointmentDTO(string DoctorName, string HospitalName, DateTime Time);
