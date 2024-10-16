using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

    public async Task DeleteDoctorRecordsFromTimetable(string doctorId)
    {
        await _dataContext.Timetables
            .Where(i => i.DoctorId == doctorId)
            .ExecuteDeleteAsync();
    }

    public async Task DeleteHospitalRecordsFromTimetable(int hospitalId)
    {
        await _dataContext.Timetables
            .Where(i => i.HospitalId == hospitalId)
            .ExecuteDeleteAsync();
    }

    public async Task DeleteRecordFromTimetable(int id)
    {
        await _dataContext.Timetables
            .Where(i => i.Id == id)
            .ExecuteDeleteAsync();
    }

    public Task<List<GetTimetableRecordDTO>> GetDoctorTimetable(string doctorId, string from, string to)
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

    public async Task UpdateRecordFromTimetable(int id, UpdateTimetableRecordDTO dto)
    {
        var timetableRecord = await
            _dataContext.Timetables
                .Include(i => i.Appointments)
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync()
                ?? throw new ApplicationException($"Запись расписания с id - {id} не найдена");

        if (timetableRecord.Appointments.Where(i => i.UserId != null).Any())
        {
            throw new ApplicationException($"Уже есть записавшиеся на приём, id - {id}");
        }

        _mapper.Map(dto, timetableRecord);

        using var tr = _dataContext.Database.BeginTransaction();

        _dataContext.Update(timetableRecord);
        await _dataContext.SaveChangesAsync();

        // Убираем старые записи на приём
        foreach (var room in timetableRecord.Appointments.ToList())
        {
            _dataContext.Appointments.Remove(room);
            timetableRecord.Appointments.Remove(room);
        }

        // Добавляем записи на приём
        for (var i = dto.From; i < dto.To; i = i.AddMinutes(30))
        {
            var newAppointment = new Appointment() { Time = i };

            _dataContext.Appointments.Add(newAppointment);
            timetableRecord.Appointments.Add(newAppointment);
        }

        await _dataContext.SaveChangesAsync();

        await tr.CommitAsync();
    }
}
