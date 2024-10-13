using TimetableService.Models.Appointments;

namespace TimetableService.Models.Timetables;

public class Timetable
{
    public int Id { get; set; }

    public int HospitalId { get; set; }

    public string DoctorId { get; set; } = null!;

    public DateTime From { get; set; }

    public DateTime To { get; set; }

    public string Room { get; set; } = null!;

    public virtual List<Appointment> Appointments { get; set; } = new();
}
