public class ProxyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HttpClient _httpClient;
    private readonly Uri _agentURL;

    public ProxyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // NOT IN PRODUCTION
        };
        _httpClient = new HttpClient(handler);
        _agentURL = new Uri(configuration.GetValue<string>("AgentURL"));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var remoteIpAddress = context.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.Request.Headers["User-Agent"];
        var requestMethod = context.Request.Method;
        var requestPath = context.Request.Path;

        Console.WriteLine($"Request from IP: {remoteIpAddress}");
        Console.WriteLine($"User-Agent: {userAgent}");
        Console.WriteLine($"HTTP Method: {requestMethod}");
        Console.WriteLine($"Request Path: {requestPath}");

        if (requestPath.StartsWithSegments("/proxy"))
        {
            var targetUri = new Uri(_agentURL, requestPath.Value.Substring(6));

            var requestMessage = new HttpRequestMessage();

            if (!HttpMethods.IsGet(requestMethod) && !HttpMethods.IsHead(requestMethod))
            {
                var streamContent = new StreamContent(context.Request.Body);
                requestMessage.Content = streamContent;
            }

            foreach (var header in context.Request.Headers)
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }

            requestMessage.RequestUri = targetUri;
            requestMessage.Method = new HttpMethod(requestMethod);

            var responseMessage = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);

            context.Response.StatusCode = (int)responseMessage.StatusCode;
            foreach (var header in responseMessage.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            context.Response.Headers.Remove("transfer-encoding");

            await responseMessage.Content.CopyToAsync(context.Response.Body);
        }
        else
        {
            await _next(context);
        }
    }
}
