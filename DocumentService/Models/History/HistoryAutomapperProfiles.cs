using AutoMapper;
using DocumentService.Models.History.DTO;

namespace DocumentService.Models.History;

public class HistoryAutomapperProfiles : Profile
{
    public HistoryAutomapperProfiles()
    {
        CreateMap<History, GetHistoryRecordDTO>();
        CreateMap<AddHistoryRecordDTO, History>();
        CreateMap<UpdateHistoryRecordDTO, History>();
    }
}
