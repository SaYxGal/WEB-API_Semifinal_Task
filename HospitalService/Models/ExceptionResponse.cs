using System.Net;

namespace HospitalService.Models;

public record ExceptionResponse(HttpStatusCode StatusCode, string Description, string? InnerException = "");
