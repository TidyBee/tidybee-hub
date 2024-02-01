using auth.Models;
using Microsoft.AspNetCore.Mvc;
using auth.Repository;

namespace auth.Controllers;

[ApiController]
[Route("[controller]")]
public class AOTHController : ControllerBase
{
    private readonly AgentRepository _agentRepository;

    public AOTHController(AgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    [HttpPost]
    public IActionResult InitAgent([FromBody] AgentMetadataModel metadata)
    {
        AgentModel newAgent = new()
        {
            Status = AgentStatusModel.Connected,
            LastPing = DateTime.Now,
            Metadata = metadata,
            ConnectionInformation = new ConnectionModel { Port = (uint)(Request.Host.Port ?? 80), Address = Request.Host.Host }
        };

        _agentRepository.AddAgent(newAgent);
        return Ok(newAgent.Uuid);
    }

    [HttpPost("{id}/{ping?}")]
    public IActionResult ConnectAgent(Guid id, string? ping, [FromBody] AgentMetadataModel? metadata)
    {
        AgentModel? agent = _agentRepository.GetAgentById(id, false, true);

        if (agent == null)
            return Unauthorized("Authentication failed.");

        if (ping != null && ping != "ping")
            return BadRequest(ping + " action not valid.");

        AgentModel updatedAgent = new()
        {
            Uuid = id,
            Status = AgentStatusModel.Connected,
            LastPing = DateTime.Now,
            Metadata = agent.ConnectionInformation != null ? FetchMetadataFromExternalUrl(agent.ConnectionInformation) : metadata,
            ConnectionInformation = agent.ConnectionInformation ?? new ConnectionModel { Port = (uint)(Request.Host.Port ?? 80), Address = Request.Host.Host }
        };

        if (updatedAgent.Metadata!.Json == "")
        {
            updatedAgent.Status = agent.Status == AgentStatusModel.Disconnected ? AgentStatusModel.Disconnected : AgentStatusModel.TroubleShooting;
            updatedAgent.LastPing = agent.LastPing;
        }

        _agentRepository.UpdateAgent(updatedAgent);
        return Ok();
    }

    [HttpDelete("{id}/disconnect")]
    public IActionResult DisconnectAgent(Guid id, [FromBody] AgentMetadataModel? metadata)
    {
        AgentModel? agent = _agentRepository.GetAgentById(id, false, true);

        if (agent == null)
            return Unauthorized("Authentication failed.");

        if (agent.Status == AgentStatusModel.Disconnected)
            return BadRequest("Agent already disconnected.");

        AgentModel updatedAgent = new()
        {
            Uuid = id,
            Status = AgentStatusModel.Disconnected,
            LastPing = DateTime.Now,
            Metadata = agent.ConnectionInformation != null ? FetchMetadataFromExternalUrl(agent.ConnectionInformation) : metadata,
            ConnectionInformation = agent.ConnectionInformation ?? new ConnectionModel { Port = (uint)(Request.Host.Port ?? 80), Address = Request.Host.Host }
        };

        _agentRepository.UpdateAgent(updatedAgent);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteAgent(Guid id, [FromBody] AgentMetadataModel? metadata)
    {
        AgentModel? agent = _agentRepository.GetAgentById(id, false, true);

        if (agent == null)
            return Unauthorized("Authentication failed.");

        AgentModel updatedAgent = new()
        {
            Uuid = id,
            Status = AgentStatusModel.Deleted,
            LastPing = DateTime.Now,
            Metadata = agent.ConnectionInformation != null ? FetchMetadataFromExternalUrl(agent.ConnectionInformation) : metadata,
            ConnectionInformation = agent.ConnectionInformation ?? new ConnectionModel { Port = (uint)(Request.Host.Port ?? 80), Address = Request.Host.Host }
        };

        _agentRepository.UpdateAgent(updatedAgent);
        return Ok();
    }

    private AgentMetadataModel? FetchMetadataFromExternalUrl(ConnectionModel connectionInformation)
    {
        using HttpClient client = new();
        string getUrl = $"http://{connectionInformation.Address}:{connectionInformation.Port}/get_status";
        HttpResponseMessage response = client.GetAsync(getUrl).Result;
        string jsonContent = response.Content.ReadAsStringAsync().Result;
        AgentMetadataModel metadata = new() { Json = jsonContent };
        return metadata;
    }
}
