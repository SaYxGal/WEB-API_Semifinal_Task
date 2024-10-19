using System.Net;

namespace DocumentService.Models;

public record ExceptionResponse(HttpStatusCode StatusCode, string Description, string? InnerException = "");
