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

    public IEnumerable<AgentModel> GetAllAgents()
    {
        return _dbContext.Agents.ToList();
    }

    public AgentModel? GetAgentById(Guid uuid)
    {
        return _dbContext.Agents.FirstOrDefault(a => a.Uuid == uuid);
    }

    public void AddAgent(AgentModel agent)
    {
        _dbContext.Agents.Add(agent);
        _dbContext.SaveChanges();
    }
}
