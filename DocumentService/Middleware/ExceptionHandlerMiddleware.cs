using DocumentService.Models;
using System.Net;

namespace DocumentService.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        ExceptionResponse response = ex switch
        {
            ApplicationException _ => new ExceptionResponse(HttpStatusCode.BadRequest, ex.Message),
            KeyNotFoundException _ => new ExceptionResponse(HttpStatusCode.NotFound, ex.Message),
            UnauthorizedAccessException _ => new ExceptionResponse(HttpStatusCode.Unauthorized, ex.Message),
            _ => new ExceptionResponse(HttpStatusCode.InternalServerError, ex.Message, ex.InnerException?.Message)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}
