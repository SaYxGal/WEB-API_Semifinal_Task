using AutoMapper;
using AutoMapper.QueryableExtensions;
using HospitalService.Data;
using HospitalService.Models.Hospitals;
using HospitalService.Models.Hospitals.DTO;
using HospitalService.Models.Rooms;
using Microsoft.EntityFrameworkCore;

namespace HospitalService.Services.Hospitals;

public class HospitalServiceImpl : IHospitalService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public HospitalServiceImpl(DataContext dataContext, IMapper mapper)
    {
        _dataContext = dataContext;
        _mapper = mapper;
    }

    public async Task Create(CreateHospitalDTO dto)
    {
        var hospital = new Hospital()
        {
            Rooms = dto.Rooms.Distinct().Select(i => new Room { Name = i }).ToList()
        };

        _mapper.Map(dto, hospital);

        _dataContext.Hospitals.Add(hospital);

        await _dataContext.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var hospital = await Find(id);

        hospital.IsDeleted = true;

        _dataContext.Update(hospital);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<GetHospitalDTO> GetById(int id)
    {
        return _mapper.Map<Hospital, GetHospitalDTO>(await Find(id));
    }

    public async Task<List<string>> GetHospitalRooms(int id)
    {
        var hospital = await
            _dataContext.Hospitals
                .Include(i => i.Rooms)
                .Where(i => i.Id == id && !i.IsDeleted)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException();

        return hospital.Rooms.Select(i => i.Name).ToList();
    }

    public async Task<List<GetHospitalDTO>> GetHospitals(int from, int count)
    {
        return await
            _dataContext.Hospitals
                .Where(x => !x.IsDeleted)
                .Skip(from - 1).Take(count)
                .ProjectTo<GetHospitalDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
    }

    public async Task Update(int id, UpdateHospitalDTO dto)
    {
        var hospital = await
            _dataContext.Hospitals
                .Include(i => i.Rooms)
                .Where(i => i.Id == id && !i.IsDeleted)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException();

        _mapper.Map(dto, hospital);

        using var tr = _dataContext.Database.BeginTransaction();

        _dataContext.Update(hospital);
        await _dataContext.SaveChangesAsync();

        // Убираем удаленные комнаты
        foreach (var room in hospital.Rooms.ToList())
        {
            if (!dto.Rooms.Any(i => i == room.Name))
            {
                _dataContext.Rooms.Remove(room);
                hospital.Rooms.Remove(room);
            }
            else
            {
                dto.Rooms.Remove(room.Name);
            }
        }

        // Добавляем недостающие комнаты
        foreach (var room in dto.Rooms.Distinct())
        {
            var objRoom = new Room { Name = room };

            _dataContext.Rooms.Add(objRoom);
            hospital.Rooms.Add(objRoom);
        }

        await _dataContext.SaveChangesAsync();

        await tr.CommitAsync();
    }

    private async Task<Hospital> Find(int id)
    {
        return await
            _dataContext.Hospitals
                .Where(i => i.Id == id && !i.IsDeleted)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException();
    }
}
