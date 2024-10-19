using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentService.Configuration;
using DocumentService.Data;
using DocumentService.Models;
using DocumentService.Models.History;
using DocumentService.Models.History.DTO;
using DocumentService.Models.Users.DTO;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace DocumentService.Services.Histories;

public class HistoryService : IHistoryService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClientRequestUri _httpClientRequestUri;
    private readonly AppServices _services;

    public HistoryService(DataContext dataContext, IMapper mapper, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, HttpClientRequestUri httpClientRequestUri, AppServices services)
    {
        _dataContext = dataContext;
        _mapper = mapper;
        _httpClient = httpClientFactory.CreateClient();
        _httpClientRequestUri = httpClientRequestUri;
        _services = services;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task CreateHistoryRecord(AddHistoryRecordDTO dto)
    {
        if (!await DoesDoctorExists(dto.DoctorId))
        {
            throw new KeyNotFoundException("Нет такого доктора");
        }

        if (!await DoesHospitalRoomExists(dto.HospitalId, dto.Room))
        {
            throw new KeyNotFoundException("Нет такого кабинета в больнице");
        }

        if (!await DoesPacientExists(dto.PacientId))
        {
            throw new KeyNotFoundException("Не существует пациента с таким идентификатором");
        }

        var historyRecord = new History();

        _mapper.Map(dto, historyRecord);

        _dataContext.Histories.Add(historyRecord);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<List<GetHistoryRecordDTO>> GetAccountHistory(string accountId)
    {
        var isAllowedFlag = false;

        if (accountId != _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString())
        {
            if (_httpContextAccessor.HttpContext?.Items["Roles"] is List<string> userRoles)
            {
                if (userRoles.Any(i => i == UserRole.Doctor))
                {
                    isAllowedFlag = true;
                }
                else
                {
                    isAllowedFlag = false;
                }
            }
            else
            {
                isAllowedFlag = false;
            }
        }
        else
        {
            isAllowedFlag = true;
        }


        if (isAllowedFlag)
        {
            return await
                _dataContext.Histories
                    .Where(i => i.PacientId == accountId)
                    .ProjectTo<GetHistoryRecordDTO>(_mapper.ConfigurationProvider)
                    .ToListAsync();
        }
        else
        {
            throw new ApplicationException("Нет доступа");
        }
    }

    public async Task<GetHistoryRecordDTO> GetHistoryRecord(int id)
    {
        var historyRecord = await
            _dataContext.Histories
                .Where(i => i.Id == id)
                .ProjectTo<GetHistoryRecordDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException($"Записи в истории с id - {id} не найдено");

        var isAllowedFlag = false;

        if (historyRecord.PacientId != _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString())
        {
            if (_httpContextAccessor.HttpContext?.Items["Roles"] is List<string> userRoles)
            {
                if (userRoles.Any(i => i == UserRole.Doctor))
                {
                    isAllowedFlag = true;
                }
                else
                {
                    isAllowedFlag = false;
                }
            }
            else
            {
                isAllowedFlag = false;
            }
        }
        else
        {
            isAllowedFlag = true;
        }


        if (isAllowedFlag)
        {
            return historyRecord;
        }
        else
        {
            throw new ApplicationException("Нет доступа");
        }
    }

    public async Task UpdateHistoryRecord(int id, UpdateHistoryRecordDTO dto)
    {
        if (!await DoesDoctorExists(dto.DoctorId))
        {
            throw new KeyNotFoundException("Нет такого доктора");
        }

        if (!await DoesHospitalRoomExists(dto.HospitalId, dto.Room))
        {
            throw new KeyNotFoundException("Нет такого кабинета в больнице");
        }

        if (!await DoesPacientExists(dto.PacientId))
        {
            throw new KeyNotFoundException("Не существует пациента с таким идентификатором");
        }

        var historyRecord = await
            _dataContext.Histories
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException($"Запись в истории с id - {id} не найдена");

        _mapper.Map(dto, historyRecord);

        _dataContext.Update(historyRecord);
        await _dataContext.SaveChangesAsync();
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

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }

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

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }

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

    private async Task<bool> DoesPacientExists(string pacientId)
    {
        var pacientRequest = new HttpRequestMessage()
        {
            RequestUri = new Uri(_services.AuthService + _httpClientRequestUri.GetUser.Replace("{id}", pacientId.ToString())),
            Method = HttpMethod.Get,
        };

        pacientRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext?.Items["Token"]?.ToString());

        var response = await _httpClient.SendAsync(pacientRequest);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }

        if (response.IsSuccessStatusCode)
        {
            var user = JsonConvert.DeserializeObject<GetUserDTO>(await response.Content.ReadAsStringAsync());

            if (user == null || !user.Roles.Where(i => i == UserRole.User).Any())
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
}
