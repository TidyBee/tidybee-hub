using System.Text.Json;
using ApiGateway.Models;

namespace ApiGateway
{
    public class AgentsHandling
    {
        private readonly List<AgentModel> _agents = new();
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public AgentsHandling(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task UpdateConnectedAgentsAsync()
        {
            var response = await _httpClient.GetAsync("http://hub-api-gateway/gateway/auth/agent/connected?includeConnectionInformation=true");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var agentModel = JsonSerializer.Deserialize<List<AgentModel>>(content);
                foreach (var agent in from agent in agentModel
                                      where agent.Status == AgentStatusModel.Connected
                                      select agent)
                {
                    _logger.LogInformation($"Agent {agent.Uuid} is connected");
                    _logger.LogInformation($"Agent {agent}");
                    _agents.Add(agent);
                }
            }
        }

        public List<AgentModel> GetConnectedAgents()
        {
            return _agents;
        }
    };
}