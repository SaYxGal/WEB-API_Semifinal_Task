using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TimetableService.Configuration;
using TimetableService.Data;
using TimetableService.Models.Appointments;
using TimetableService.Models.Timetables;
using TimetableService.Models.Timetables.DTO;

namespace TimetableService.Services.Timetables;

public class TimetableServiceImpl : ITimetableService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClientRequestUri _httpClientRequestUri;
    private readonly AppServices _services;

    public TimetableServiceImpl(DataContext dataContext, IMapper mapper, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, HttpClientRequestUri httpClientRequestUri, AppServices services)
    {
        _dataContext = dataContext;
        _mapper = mapper;
        _httpClient = httpClientFactory.CreateClient();
        _httpContextAccessor = httpContextAccessor;
        _httpClientRequestUri = httpClientRequestUri;
        _services = services;
    }

    public async Task AddRecordToTimetable(AddTimetableRecordDTO dto)
    {
        if (!(await DoesDoctorExists(dto.DoctorId)))
        {
            throw new ApplicationException("Нет такого доктора");
        }

        if (!(await DoesHospitalRoomExists(dto.HospitalId, dto.Room)))
        {
            throw new ApplicationException("Нет такого кабинета в больнице");
        }

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
        if (!(await DoesDoctorExists(doctorId)))
        {
            throw new ApplicationException("Нет такого доктора");
        }

        await _dataContext.Timetables
            .Where(i => i.DoctorId == doctorId)
            .ExecuteDeleteAsync();
    }

    public async Task DeleteHospitalRecordsFromTimetable(int hospitalId)
    {
        if (!(await DoesHospitalExists(hospitalId)))
        {
            throw new ApplicationException("Нет такой больницы");
        }

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

    public async Task<List<GetTimetableRecordDTO>> GetDoctorTimetable(string doctorId, string from, string to)
    {
        if (!(await DoesDoctorExists(doctorId)))
        {
            throw new ApplicationException("Нет такого доктора");
        }

        return await
            _dataContext.Timetables
                .Where(x => x.DoctorId == doctorId
                    && x.From >= DateTime.Parse(from)
                    && x.To <= DateTime.Parse(to))
                .ProjectTo<GetTimetableRecordDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
    }

    public async Task<List<GetTimetableRecordDTO>> GetHospitalRoomTimetable(int hospitalId, string room, string from, string to)
    {
        if (!(await DoesHospitalRoomExists(hospitalId, room)))
        {
            throw new ApplicationException("Нет такого кабинета в больнице");
        }

        return await
            _dataContext.Timetables
                .Where(x => x.HospitalId == hospitalId && x.Room == room
                    && x.From >= DateTime.Parse(from)
                    && x.To <= DateTime.Parse(to))
                .ProjectTo<GetTimetableRecordDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
    }

    public async Task<List<GetTimetableRecordDTO>> GetHospitalTimetable(int hospitalId, string from, string to)
    {
        if (!(await DoesHospitalExists(hospitalId)))
        {
            throw new ApplicationException("Нет такой больницы");
        }

        return await
            _dataContext.Timetables
                .Where(x => x.HospitalId == hospitalId
                    && x.From >= DateTime.Parse(from)
                    && x.To <= DateTime.Parse(to))
                .ProjectTo<GetTimetableRecordDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
    }

    public async Task UpdateRecordFromTimetable(int id, UpdateTimetableRecordDTO dto)
    {
        if (!(await DoesDoctorExists(dto.DoctorId)))
        {
            throw new ApplicationException("Нет такого доктора");
        }

        if (!(await DoesHospitalRoomExists(dto.HospitalId, dto.Room)))
        {
            throw new ApplicationException("Нет такого кабинета в больнице");
        }

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

    private async Task<bool> DoesDoctorExists(string doctorId)
    {
        var doctorRequest = new HttpRequestMessage()
        {
            RequestUri = new Uri(_services.AuthService + _httpClientRequestUri.GetDoctor.Replace("{id}", doctorId)),
            Method = HttpMethod.Get,
        };

        doctorRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext?.Items["Token"]?.ToString());

        var response = await _httpClient.SendAsync(doctorRequest);

        return response.IsSuccessStatusCode;
    }

    private async Task<bool> DoesHospitalRoomExists(int hospitalId, string room)
    {
        var hospitalRequest = new HttpRequestMessage()
        {
            RequestUri = new Uri(_services.HospitalService + _httpClientRequestUri.GetHospitalRooms.Replace("{id}", hospitalId.ToString())),
            Method = HttpMethod.Get,
        };

        hospitalRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext?.Items["Token"]?.ToString());

        var response = await _httpClient.SendAsync(hospitalRequest);

        if (response.IsSuccessStatusCode)
        {
            var rooms = JsonConvert.DeserializeObject<List<string>>(await response.Content.ReadAsStringAsync());

            if (rooms == null || !rooms.Where(i => i == room).Any())
            {
                return false;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task<bool> DoesHospitalExists(int hospitalId)
    {
        var hospitalRequest = new HttpRequestMessage()
        {
            RequestUri = new Uri(_services.HospitalService + _httpClientRequestUri.GetHospital.Replace("{id}", hospitalId.ToString())),
            Method = HttpMethod.Get,
        };

        hospitalRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext?.Items["Token"]?.ToString());

        var response = await _httpClient.SendAsync(hospitalRequest);

        return response.IsSuccessStatusCode;
    }
}
