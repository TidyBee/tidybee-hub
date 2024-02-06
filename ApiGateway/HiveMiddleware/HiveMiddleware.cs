using ApiGateway.Models;
using Microsoft.AspNetCore.Http.Extensions;

namespace ApiGateway
{
    public class HiveMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;
        private readonly Uri? _agentURL;
        private List<AgentModel> _connectedAgents;

        public HiveMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // NOT IN PRODUCTION
            };
            _httpClient = new HttpClient(handler);
            var url = configuration.GetValue<string>("AgentURL"); /// CHANGE THAT BY QUERYING /gateway/auth/getAllAgent -> Keep only connected one and extract from them connectionInformation
            _connectedAgents = new List<AgentModel>();
            if (!string.IsNullOrEmpty(url))
            {
                _agentURL = new Uri(url);
            } else {
                _agentURL = null;
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestPath = context.Request.Path;
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();


            if (requestPath.StartsWithSegments("/proxy"))
            {
                LogHive(context);
                var AgentsHandling = new AgentsHandling(_httpClient, logger);
                await AgentsHandling.UpdateConnectedAgentsAsync();
                _connectedAgents = AgentsHandling.GetConnectedAgents();
                if (!_connectedAgents.Any())
                {
                    context.Response.StatusCode = 503;
                    await context.Response.WriteAsync("No agent available");
                    return;
                } else {
                    foreach (var agent in _connectedAgents)
                    {
                        if (agent.ConnectionInformation != null)
                        {
                            var agentURL = new Uri($"http://{agent.ConnectionInformation.Address}:{agent.ConnectionInformation.Port}");
                            logger.LogInformation($"Proxying to {agentURL}");
                            await ProxyRequest(context, agentURL, logger);
                            // await _httpClient.GetAsync($"http://hub-api-gateway/gateway/auth/{agent.Uuid}/ping");
                        }
                    }
                }
            }
            else
            {
                await _next(context);
            }
            /// AT THE END OF THE PROXY / IN THE MEAN TIME QUERY /gateway/auth/<agentID>/ping to update his metadata
        }

        public async Task ProxyRequest(HttpContext context, Uri agentURL, ILogger logger) {
            var requestMethod = context.Request.Method;
            var targetUri = new Uri(agentURL, context.Request.GetDisplayUrl().Split('/')[^1]);

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

            logger.LogInformation($"Body: {responseMessage.Content.ReadAsStringAsync().Result}");
            await responseMessage.Content.CopyToAsync(context.Response.Body);
        }

        public static void LogHive(HttpContext context)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            logger.LogInformation($"Method: {context.Request.Method}, URL: {context.Request.GetDisplayUrl()}");
            foreach (var header in context.Request.Headers)
            {
                logger.LogInformation($"Header: {header.Key}, Value: {header.Value}");
            }

            if (context.Request.ContentLength > 0 && (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put))
            {
                context.Request.EnableBuffering();
                var bodyReader = new StreamReader(context.Request.Body);
                var bodyContent = bodyReader.ReadToEndAsync().Result;
                context.Request.Body.Position = 0;
                logger.LogInformation($"Body: {bodyContent}");
            }
        }
    }
}