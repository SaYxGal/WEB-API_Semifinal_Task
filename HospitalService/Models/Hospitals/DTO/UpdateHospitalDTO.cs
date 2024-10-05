namespace HospitalService.Models.Hospitals.DTO;

public record UpdateHospitalDTO(string Name, string Address, string ContactPhone, List<string> Rooms);
