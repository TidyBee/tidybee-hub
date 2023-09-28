public class ProxyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HttpClient _httpClient;

    public ProxyMiddleware(RequestDelegate next)
    {
        _next = next;
        _httpClient = new HttpClient();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string remoteIpAddress = context.Connection.RemoteIpAddress?.ToString();
        string userAgent = context.Request.Headers["User-Agent"];
        string requestMethod = context.Request.Method;
        string requestPath = context.Request.Path;

        Console.WriteLine($"Request from IP: {remoteIpAddress}");
        Console.WriteLine($"User-Agent: {userAgent}");
        Console.WriteLine($"HTTP Method: {requestMethod}");
        Console.WriteLine($"Request Path: {requestPath}");

        await _next(context);
    }
}
