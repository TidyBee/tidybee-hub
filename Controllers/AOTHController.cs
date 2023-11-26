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
            ConnectionInformation = new ConnectionModel { Port = 1, Address = "" }
        };

        _agentRepository.AddAgent(newAgent);
        return Ok(newAgent.Uuid);
    }
}
