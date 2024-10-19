using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TimetableService.Data;
using TimetableService.Models;
using TimetableService.Models.Appointments.DTO;

namespace TimetableService.Services.Appointments;

public class AppointmentService : IAppointmentService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppointmentService(DataContext dataContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _dataContext = dataContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<GetAppointmentDTO>> GetFreeAppointments(int timetableId)
    {
        return await
            _dataContext.Appointments
                .Where(i => i.TimetableId == timetableId && i.UserId == null)
                .ProjectTo<GetAppointmentDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
    }

    public async Task AssignUserToAppointment(int timetableId, DateTime time)
    {
        var appointment = await
            _dataContext.Appointments
                .Where(i => i.TimetableId == timetableId && i.Time == time)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Запись не найдена");

        if (appointment.UserId != null)
        {
            throw new ApplicationException("Указанное время уже занято");
        }

        appointment.UserId = _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();

        _dataContext.Update(appointment);
        await _dataContext.SaveChangesAsync();
    }

    public async Task UnassignUser(int id)
    {
        var appointment = await
            _dataContext.Appointments
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Запись не найдена");

        if (appointment.UserId != _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString())
        {
            if (_httpContextAccessor.HttpContext?.Items["Roles"] is List<string> userRoles)
            {
                if (userRoles.Any(i => i == UserRole.Admin || i == UserRole.Manager))
                {
                    appointment.UserId = null;
                }
                else
                {
                    throw new ApplicationException("Отменить запись может только записавшийся пользователь, администратор или менеджер");
                }
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }
        else
        {
            appointment.UserId = null;
        }

        _dataContext.Update(appointment);
        await _dataContext.SaveChangesAsync();
    }
}
