using AutoMapper;
using TimetableService.Data;
using TimetableService.Models.Appointments;
using TimetableService.Models.Timetables;
using TimetableService.Models.Timetables.DTO;

namespace TimetableService.Services.Timetables;

public class TimetableServiceImpl : ITimetableService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public TimetableServiceImpl(DataContext dataContext, IMapper mapper)
    {
        _dataContext = dataContext;
        _mapper = mapper;
    }

    public async Task AddRecordToTimetable(AddTimetableRecordDTO dto)
    {
        var timetableRecord = new Timetable();

        _mapper.Map(dto, timetableRecord);

        var emptyAppointments = new List<Appointment>();

        for (var i = dto.From; i < dto.To; i = i.AddMinutes(30))
        {
            emptyAppointments.Add(new Appointment() { Time = i });
        }

        timetableRecord.Appointments = emptyAppointments;

        _dataContext.Timetables.Add(timetableRecord);

        await _dataContext.SaveChangesAsync();
    }

    public Task DeleteDoctorRecordsFromTimetable(int doctorId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteHospitalRecordsFromTimetable(int hospitalId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteRecordFromTimetable(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<GetTimetableRecordDTO>> GetDoctorTimetable(int doctorId, string from, string to)
    {
        throw new NotImplementedException();
    }

    public Task<List<GetTimetableRecordDTO>> GetHospitalRoomTimetable(int hospitalId, int roomId, string from, string to)
    {
        throw new NotImplementedException();
    }

    public Task<List<GetTimetableRecordDTO>> GetHospitalTimetable(int hospitalId, string from, string to)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRecordFromTimetable(int id, UpdateTimetableRecordDTO dto)
    {
        throw new NotImplementedException();
    }
}
