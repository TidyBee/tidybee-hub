using api.Models;
using Microsoft.AspNetCore.Mvc;
using tidybee_hub.Repository;

namespace tidybee_hub.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    [HttpPost("{id}")]
    public IActionResult ConnectAgent(Guid id, [FromBody] AgentMetadataModel metadata)
    {
        AgentModel? agent = _agentRepository.GetAgentById(id);

        if (agent == null)
            return Unauthorized("Authentication failed.");

        AgentModel updatedAgent = new()
        {
            Uuid = id,
            Status = AgentStatusModel.Connected,
            LastPing = DateTime.Now,
            Metadata = metadata,
            ConnectionInformation = new ConnectionModel { Port = (uint)(Request.Host.Port ?? 80), Address = Request.Host.Host }
        };

        _agentRepository.UpdateAgent(updatedAgent);
        return Ok();
    }
}
