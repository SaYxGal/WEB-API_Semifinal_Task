namespace HospitalService.Models.Hospitals.DTO;

public record CreateHospitalDTO(string Name, string Address, string ContactPhone, List<string> Rooms);
