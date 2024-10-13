using AutoMapper;
using TimetableService.Models.Timetables.DTO;

namespace TimetableService.Models.Timetables;

public class TimetableServiceAutomapperProfiles : Profile
{
    public TimetableServiceAutomapperProfiles()
    {
        CreateMap<Timetable, GetTimetableRecordDTO>();
        CreateMap<AddTimetableRecordDTO, Timetable>();
        CreateMap<UpdateTimetableRecordDTO, Timetable>();
    }
}
