using ApiGateway.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace ApiGateway
{
    public class HiveMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;
        private readonly string? _gatewayUrl;

        public HiveMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _gatewayUrl = configuration.GetValue<string>("GatewayServiceUrl");
            if (_gatewayUrl == null)
            {
                throw new ArgumentNullException("GatewayServiceUrl is not set in configuration.");
            }
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // NOT IN PRODUCTION
            };
            _httpClient = new HttpClient(handler);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestPath = context.Request.Path;
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            var configuration = context.RequestServices.GetRequiredService<IConfiguration>();

            if (requestPath.StartsWithSegments("/proxy"))
            {
                LogHive(context);
                var AgentsHandling = new AgentsHandling(_httpClient, logger, configuration);
                await AgentsHandling.UpdateConnectedAgentsAsync();
                var connectedAgents = AgentsHandling.GetConnectedAgents();
                var responses = new List<HiveResponseModel>();
                var contentStringBuilder = new StringBuilder();

                if (!connectedAgents.Any())
                {
                    responses.Add(new HiveResponseModel
                    {
                        StatusCode = 503,
                        Content = "No agent available"
                    });
                } else {
                    foreach (var agent in connectedAgents)
                    {
                        if (agent.ConnectionInformation != null)
                        {
                            var agentURL = new Uri($"http://{agent.ConnectionInformation.Address}:{agent.ConnectionInformation.Port}");
                            logger.LogInformation($"Proxying to {agentURL}");
                            var responseModel = await ProxyRequestAsync(context, agentURL, logger);
                            logger.LogInformation($"Response: {responseModel.StatusCode}");
                            responses.Add(responseModel);
                            contentStringBuilder.AppendLine(responseModel.Content);
                            await _httpClient.GetAsync($"{_gatewayUrl}/gateway/auth/aoth/{agent.Uuid}/ping");
                        }
                    }
                }
                var jsonResponse = new HiveJsonResponse
                {
                    Responses = responses
                };
                var json = JsonConvert.SerializeObject(jsonResponse);

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }
            else
            {
                await _next(context);
            }
        }

        public async Task<HiveResponseModel> ProxyRequestAsync(HttpContext context, Uri agentURL, ILogger logger) {
            var requestMethod = context.Request.Method;
            var targetUri = new Uri(agentURL, context.Request.GetDisplayUrl().Split('/')[^1]);
            var requestMessage = new HttpRequestMessage();

            try
            {
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

                var responseModel = new HiveResponseModel
                {
                    StatusCode = (int)responseMessage.StatusCode,
                    Content = await responseMessage.Content.ReadAsStringAsync()
                };

                logger.LogInformation($"Body: {responseModel.Content}");
                return responseModel;

            } catch (Exception e) {
                logger.LogError(e, $"Error proxying request to agent: {agentURL}");
                var responseModel = new HiveResponseModel
                {
                    StatusCode = 500,
                    Content = $"Error proxying request to agent: {agentURL}"
                };
                return responseModel;
            }
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