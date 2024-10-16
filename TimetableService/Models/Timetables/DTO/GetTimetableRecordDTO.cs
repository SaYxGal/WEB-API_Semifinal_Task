namespace TimetableService.Models.Timetables.DTO;

public record GetTimetableRecordDTO(int HospitalId, string DoctorId, DateTime From, DateTime To, string Room);
