using AuthenticationService.Data;
using AuthenticationService.Models.Users;
using AuthenticationService.Models.Users.DTO;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Services.Doctor;

public class DoctorService : IDoctorService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public DoctorService(DataContext dataContext, IMapper mapper)
    {
        _dataContext = dataContext;
        _mapper = mapper;
    }

    public async Task<GetUserDTO> GetById(string id)
    {
        var doctor = await
            GetDoctorsQuery()
                .Where(i => i.Id == id)
                .ProjectTo<GetUserDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new ApplicationException($"Доктор с id {id} не найден");

        doctor.Roles =
            (
                from r in _dataContext.Roles
                join ur in _dataContext.UserRoles on r.Id equals ur.RoleId
                where ur.UserId == id
                select r.Name
            ).ToList();

        return doctor;
    }

    public async Task<List<GetUserListDTO>> GetDoctors(string? nameFilter, int from, int count)
    {
        var list = GetDoctorsQuery();

        if (!string.IsNullOrEmpty(nameFilter))
        {
            var term = "%" + nameFilter + "%";

            list = list.Where(i => EF.Functions.Like((i.FirstName + " " + i.LastName).ToLower(), term.ToLower()));
        }

        return await list
                .Skip(from - 1)
                .Take(count)
                .ProjectTo<GetUserListDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
    }

    private IQueryable<User> GetDoctorsQuery()
    {
        return
            from u in _dataContext.Users
            join ur in _dataContext.UserRoles on u.Id equals ur.UserId
            join r in _dataContext.Roles on ur.RoleId equals r.Id
            where r.Name == UserRole.Doctor
            select u;
    }
}
