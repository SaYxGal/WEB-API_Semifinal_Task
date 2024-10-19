namespace DocumentService.Configuration;

public class HttpClientRequestUri
{
    public string ValidateToken { get; set; } = null!;
    public string GetDoctor { get; set; } = null!;
    public string GetHospital { get; set; } = null!;
    public string GetUser { get; set; } = null!;
    public string GetHospitalRooms { get; set; } = null!;
}
