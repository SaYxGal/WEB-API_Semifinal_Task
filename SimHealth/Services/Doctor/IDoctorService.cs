using AuthenticationService.Models.Users.DTO;

namespace AuthenticationService.Services.Doctor;

public interface IDoctorService
{
    public Task<List<GetUserListDTO>> GetDoctors(string? nameFilter, int from, int count);

    public Task<GetUserDTO> GetById(string id);
}
