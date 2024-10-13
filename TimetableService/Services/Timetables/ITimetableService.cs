using TimetableService.Models.Timetables.DTO;

namespace TimetableService.Services.Timetables;

public interface ITimetableService
{
    public Task AddRecordToTimetable(AddTimetableRecordDTO dto);

    public Task UpdateRecordFromTimetable(int id, UpdateTimetableRecordDTO dto);

    public Task DeleteRecordFromTimetable(int id);

    public Task DeleteDoctorRecordsFromTimetable(int doctorId);

    public Task DeleteHospitalRecordsFromTimetable(int hospitalId);

    public Task<List<GetTimetableRecordDTO>> GetHospitalTimetable(int hospitalId, string from, string to);

    public Task<List<GetTimetableRecordDTO>> GetDoctorTimetable(int doctorId, string from, string to);

    public Task<List<GetTimetableRecordDTO>> GetHospitalRoomTimetable(int hospitalId, int roomId, string from, string to);
}
