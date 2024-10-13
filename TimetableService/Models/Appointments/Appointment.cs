namespace TimetableService.Models.Appointments;

public class Appointment
{
    public int TimetableId { get; set; }

    public string? UserId { get; set; }

    public DateTime Time { get; set; }
}
