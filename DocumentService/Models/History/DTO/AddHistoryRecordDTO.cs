namespace DocumentService.Models.History.DTO;

public record AddHistoryRecordDTO(DateTime Date, string PacientId, int HospitalId, string DoctorId, string Room, string Data);
