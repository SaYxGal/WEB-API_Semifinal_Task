using Newtonsoft.Json;
using TimetableService.Configuration;
using TimetableService.Models.JWT;

namespace TimetableService.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HttpClient _httpClient;
    private readonly HttpClientRequestUri _httpClientRequestUri;

    public JwtMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory, HttpClientRequestUri httpClientRequestUri)
    {
        _next = next;
        _httpClient = httpClientFactory.CreateClient("Auth");
        _httpClientRequestUri = httpClientRequestUri;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

        var queryParams = $"?accessToken={token}";
        var response = await _httpClient.GetAsync(_httpClientRequestUri.ValidateToken + queryParams);

        var result = JsonConvert.DeserializeObject<JWTTokenValidationResult>(await response.Content.ReadAsStringAsync());

        if (result?.IsValid == true && result.UserId != null)
        {
            context.Items["UserId"] = result.UserId;
            context.Items["Roles"] = result.Roles;
        }

        await _next(context);
    }
}
