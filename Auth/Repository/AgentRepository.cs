using auth.Models;
using auth.Context;
using Microsoft.EntityFrameworkCore;

namespace auth.Repository;

public class AgentRepository
{
    private readonly DatabaseContext _dbContext;

    public AgentRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<AgentModel> GetAllAgents(bool includeMetadata = false, bool includeConnectionInformation = false)
    {
        return _dbContext.Agents.Where(a => a.Status != AgentStatusModel.Deleted).Select(a => new AgentModel
        {
            Uuid = a.Uuid,
            Status = a.Status,
            LastPing = a.LastPing,
            Metadata = includeMetadata ? a.Metadata : null,
            ConnectionInformation = includeConnectionInformation ? a.ConnectionInformation : null
        }).ToList();
    }

    public IEnumerable<AgentModel> GetAllAgentsByStatus(AgentStatusModel status, bool includeMetadata = false, bool includeConnectionInformation = false)
    {
        return _dbContext.Agents.Where(a => a.Status == status).Select(a => new AgentModel
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
        return _dbContext.Agents.Where(a => a.Uuid == uuid && a.Status != AgentStatusModel.Deleted).Select(a => new AgentModel
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
        var entry = _dbContext.Entry(agent);

        if (entry.State == EntityState.Detached)
        {
            _dbContext.Agents.Add(agent);
        }
        _dbContext.SaveChanges();
    }

    public void UpdateAgent(AgentModel agent)
    {
        var existingAgent = _dbContext.Agents.Find(agent.Uuid);

        if (existingAgent != null)
        {
            _dbContext.Entry(existingAgent).State = EntityState.Detached;
            _dbContext.Entry(existingAgent).CurrentValues.SetValues(agent);
            _dbContext.Entry(existingAgent).State = EntityState.Modified;
        }
        else
        {
            _dbContext.Attach(agent);
            _dbContext.Entry(agent).State = EntityState.Modified;
        }

        _dbContext.SaveChanges();
    }

    public void DeleteAgent(AgentModel agent)
    {
        var existingAgent = _dbContext.Agents.Find(agent.Uuid);

        if (existingAgent != null)
        {
            _dbContext.Entry(existingAgent).State = EntityState.Detached;
            _dbContext.Attach(existingAgent);
            _dbContext.Entry(existingAgent).State = EntityState.Deleted;
            _dbContext.SaveChanges();
        }
    }

    public void DeleteInactiveAgent(int deletedTime, int disconnectedTime)
    {
        List<AgentModel> deletedAgentsList = GetAllAgentsByStatus(AgentStatusModel.Deleted)
            .Where(agent => agent.LastPing.AddDays(deletedTime) < DateTime.Now)
            .ToList();

        List<AgentModel> disconnectedAgentList = GetAllAgentsByStatus(AgentStatusModel.Disconnected)
            .Where(agent => agent.LastPing.AddDays(disconnectedTime) < DateTime.Now)
            .ToList();

        List<AgentModel> agentsToDelete = deletedAgentsList.Concat(disconnectedAgentList).ToList();

        foreach (var agent in agentsToDelete)
        {
            DeleteAgent(agent);
        }
    }

    public void PingAllAgentToTroubleShoothing()
    {
        List<AgentModel> agentsList = GetAllAgents(true, true).ToList();

        foreach (var agent in agentsList)
        {
            try
            {
                using HttpClient client = new();
                string getUrl = $"http://{agent.ConnectionInformation!.Address}:{agent.ConnectionInformation.Port}/get_status";
                HttpResponseMessage response = client.GetAsync(getUrl).Result;
                Console.WriteLine(response.StatusCode);
                if (!response.StatusCode.Equals(200))
                    throw new Exception();
                string jsonContent = response.Content.ReadAsStringAsync().Result;
                agent.Metadata!.Json = jsonContent;
                agent.Status = AgentStatusModel.Connected;
                agent.LastPing = DateTime.Now;
                UpdateAgent(agent);
            }
            catch (Exception)
            {
                agent.Status = AgentStatusModel.Disconnected;
                UpdateAgent(agent);
            }
        }
    }
}