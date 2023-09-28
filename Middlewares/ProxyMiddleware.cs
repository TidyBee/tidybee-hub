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
        Console.WriteLine($"Received request: {context.Request.Method} {context.Request.Path}");

        await _next(context);
    }
}
