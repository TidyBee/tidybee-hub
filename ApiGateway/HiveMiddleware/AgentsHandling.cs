using System.Text.Json;
using ApiGateway.Models;

namespace ApiGateway
{
    public class AgentsHandling
    {
        private readonly List<AgentModel> _agents = new();
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string? _gatewayUrl;

        public AgentsHandling(HttpClient httpClient, ILogger logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _gatewayUrl = configuration.GetValue<string>("GatewayServiceUrl");
            logger.LogInformation($"Gateway URL: {_gatewayUrl}");
            if (_gatewayUrl == null)
            {
                throw new ArgumentNullException("AothServiceUrl is not set in configuration.");
            }
        }

        public async Task UpdateConnectedAgentsAsync()
        {
            _agents.Clear();
            var response = await _httpClient.GetAsync($"{_gatewayUrl}/gateway/auth/agent/connected?includeMetadata=true&includeConnectionInformation=true");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var agentModel = JsonSerializer.Deserialize<List<AgentModel>>(content);
                if (agentModel != null)
                {
                    foreach (var agent in agentModel)
                    {
                        _logger.LogInformation($"Agent {agent.Uuid} is connected");
                        _agents.Add(agent);
                    }
                } else {
                    _logger.LogInformation("No connected agents");
                }
            }
        }

        public List<AgentModel> GetConnectedAgents()
        {
            return _agents;
        }
    };
}