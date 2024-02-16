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
            var response = await _httpClient.GetAsync("http://hub-api-gateway/gateway/auth/agent/connected?includeMetadata=true&includeConnectionInformation=true");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var agentModel = JsonSerializer.Deserialize<List<AgentModel>>(content);
                if (agentModel != null)
                {
                    foreach (var agent in agentModel)
                    {
                        await _httpClient.GetAsync($"http://hub-api-gateway/gateway/auth/agent/{agent.Uuid}/ping");
                        var updatedAgent = JsonSerializer.Deserialize<AgentModel>(await _httpClient.GetStringAsync($"http://hub-api-gateway/gateway/auth/agent/{agent.Uuid}"));
                        if (updatedAgent != null && updatedAgent.Status == AgentStatusModel.Connected)
                        {
                            _logger.LogInformation($"Agent {agent.Uuid} is connected");
                            _agents.Add(agent);
                        }

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