using HospitalService.Models.Hospitals.DTO;

namespace HospitalService.Services.Hospitals;

public interface IHospitalService
{
    public Task<GetHospitalDTO> GetById(int id);

    public Task<List<GetHospitalDTO>> GetHospitals(int from, int count);

    public Task<List<string>> GetHospitalRooms(int id);

    public Task Create(CreateHospitalDTO dto);

    public Task Update(int id, UpdateHospitalDTO dto);

    public Task Delete(int id);
}
