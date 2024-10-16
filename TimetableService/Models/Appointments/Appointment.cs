namespace TimetableService.Models.Appointments;

public class Appointment
{
    public int Id { get; set; }

    public int TimetableId { get; set; }

    public string? UserId { get; set; }

    public DateTime Time { get; set; }
}
