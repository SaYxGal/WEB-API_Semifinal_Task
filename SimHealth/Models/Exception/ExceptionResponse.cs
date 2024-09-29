using System.Net;

namespace AuthenticationService.Models.Exception;

public record ExceptionResponse(HttpStatusCode StatusCode, string Description, string? InnerException = "");
