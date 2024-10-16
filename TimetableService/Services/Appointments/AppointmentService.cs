using AutoMapper;
using TimetableService.Data;
using TimetableService.Models.Appointments.DTO;

namespace TimetableService.Services.Appointments;

public class AppointmentService : IAppointmentService
{
    private readonly DataContext _dataContext;
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;

    public AppointmentService(DataContext dataContext, IHttpClientFactory httpClientFactory, IMapper mapper)
    {
        _dataContext = dataContext;
        _mapper = mapper;
    }

    public Task<List<GetAppointmentDTO>> GetFreeAppointments(int timetableId)
    {
        throw new NotImplementedException();
    }
}
