namespace DocumentService.Models.History.DTO;

public record GetHistoryRecordDTO(DateTime Date, string PacientId, int HospitalId, string DoctorId, string Room, string Data);
