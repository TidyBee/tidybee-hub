using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Options;

namespace TidyEvents.Interceptors;

public class AuthInterceptorOption {
    public string HubUrl { get; set; } = string.Empty;
}

public class AuthInterceptor(ILogger<AuthInterceptor> logger, HttpClient httpClient, IOptions<AuthInterceptorOption> configuration) : Interceptor
{
    private readonly ILogger _logger = logger;
    private readonly HttpClient _httpClient = httpClient;
    private readonly IOptions<AuthInterceptorOption> _configuration = configuration;

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        if (!isDev && context.RequestHeaders.Get("Authorization") is null)
        {
            throw new RpcException(new Grpc.Core.Status(StatusCode.Unauthenticated, "Authorization header is missing."));
        }

        var token = context.RequestHeaders.Get("Authorization")?.Value.Split("Bearer ").LastOrDefault();

        if ((token is null || token == "test_auth_token") && isDev)
        {
            return await continuation(requestStream, context);
        }

        _logger.LogInformation($"Received request from agent: {token}, contacting hub at {_configuration.Value.HubUrl}.");

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration.Value.HubUrl}/auth/Agent/{token}");
        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new RpcException(new Grpc.Core.Status(StatusCode.Unauthenticated, "Invalid token."));
        }

        return await continuation(requestStream, context);
    }

}
