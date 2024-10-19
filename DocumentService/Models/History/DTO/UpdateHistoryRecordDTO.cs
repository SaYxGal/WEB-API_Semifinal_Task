namespace DocumentService.Models.History.DTO;

public record UpdateHistoryRecordDTO(DateTime Date, string PacientId, int HospitalId, string DoctorId, string Room, string Data);
