using Newtonsoft.Json;
using TimetableService.Configuration;
using TimetableService.Models.JWT;

namespace TimetableService.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HttpClient _httpClient;
    private readonly HttpClientRequestUri _httpClientRequestUri;
    private readonly AppServices _services;

    public JwtMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory, HttpClientRequestUri httpClientRequestUri, AppServices services)
    {
        _next = next;
        _httpClient = httpClientFactory.CreateClient();
        _httpClientRequestUri = httpClientRequestUri;
        _services = services;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

        var queryParams = $"?accessToken={token}";

        var validateRequest = new HttpRequestMessage()
        {
            RequestUri = new Uri(_services.AuthService + _httpClientRequestUri.ValidateToken + queryParams),
            Method = HttpMethod.Get,
        };

        var response = await _httpClient.SendAsync(validateRequest);

        var result = JsonConvert.DeserializeObject<JWTTokenValidationResult>(await response.Content.ReadAsStringAsync());

        if (result?.IsValid == true && result.UserId != null)
        {
            context.Items["UserId"] = result.UserId;
            context.Items["Roles"] = result.Roles;
            context.Items["Token"] = token;
        }

        await _next(context);
    }
}
