using api.Models;
using tidybee_hub.Context;

namespace tidybee_hub.Repository;

public class AgentRepository
{
    private readonly DatabaseContext _dbContext;

    public AgentRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<AgentModel> GetAllAgents(bool includeMetadata = false, bool includeConnectionInformation = false)
    {
        return _dbContext.Agents.Select(a => new AgentModel
        {
            Uuid = a.Uuid,
            Status = a.Status,
            LastPing = a.LastPing,
            Metadata = includeMetadata ? a.Metadata : null,
            ConnectionInformation = includeConnectionInformation ? a.ConnectionInformation : null
        }).ToList();
    }

    public AgentModel? GetAgentById(Guid uuid, bool includeMetadata = false, bool includeConnectionInformation = false)
    {
        return _dbContext.Agents.Where(a => a.Uuid == uuid).Select(a => new AgentModel
        {
            Uuid = a.Uuid,
            Status = a.Status,
            LastPing = a.LastPing,
            Metadata = includeMetadata ? a.Metadata : null,
            ConnectionInformation = includeConnectionInformation ? a.ConnectionInformation : null
        }).FirstOrDefault();
    }

    public void AddAgent(AgentModel agent)
    {
        _dbContext.Agents.Add(agent);
        _dbContext.SaveChanges();
    }
}
