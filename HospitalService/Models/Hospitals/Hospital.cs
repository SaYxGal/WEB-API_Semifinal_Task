using HospitalService.Models.Rooms;

namespace HospitalService.Models.Hospitals;

public class Hospital
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string ContactPhone { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual List<Room> Rooms { get; set; } = new();
}
