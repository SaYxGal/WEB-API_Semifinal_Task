namespace DocumentService.Models.History;

public class History
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string PacientId { get; set; } = null!;

    public int HospitalId { get; set; }

    public string DoctorId { get; set; } = null!;

    public string Room { get; set; } = null!;

    public string Data { get; set; } = null!;
}
