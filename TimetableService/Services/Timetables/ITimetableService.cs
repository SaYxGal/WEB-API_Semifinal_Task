using TimetableService.Models.Timetables.DTO;

namespace TimetableService.Services.Timetables;

public interface ITimetableService
{
    public Task AddRecordToTimetable(AddTimetableRecordDTO dto);

    public Task UpdateRecordFromTimetable(int id, UpdateTimetableRecordDTO dto);

    public Task DeleteRecordFromTimetable(int id);

    public Task DeleteDoctorRecordsFromTimetable(string doctorId);

    public Task DeleteHospitalRecordsFromTimetable(int hospitalId);

    public Task<List<GetTimetableRecordDTO>> GetHospitalTimetable(int hospitalId, string from, string to);

    public Task<List<GetTimetableRecordDTO>> GetDoctorTimetable(string doctorId, string from, string to);

    public Task<List<GetTimetableRecordDTO>> GetHospitalRoomTimetable(int hospitalId, string room, string from, string to);
}
