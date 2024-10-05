using AutoMapper;
using HospitalService.Models.Hospitals.DTO;

namespace HospitalService.Models.Hospitals;

public class HospitalAutomapperProfiles : Profile
{
    public HospitalAutomapperProfiles()
    {
        CreateMap<Hospital, GetHospitalDTO>();
        CreateMap<CreateHospitalDTO, Hospital>()
            .ForMember(i => i.Rooms, opt => opt.Ignore());
        CreateMap<UpdateHospitalDTO, Hospital>()
            .ForMember(i => i.Rooms, opt => opt.Ignore());
    }
}
