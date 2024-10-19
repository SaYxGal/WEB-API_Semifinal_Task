using System.Net;

namespace TimetableService.Models;

public record ExceptionResponse(HttpStatusCode StatusCode, string Description, string? InnerException = "");
