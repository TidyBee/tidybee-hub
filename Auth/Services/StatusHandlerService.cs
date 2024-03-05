using System;
using System.Threading;
using auth.Repository;

public class StatusHandlerService
{
    private Timer? _timer;
    private AgentRepository _agentRepository;
    private int? _statusTiming;
    private ILogger? _logger;

    public StatusHandlerService(AgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public void Start(int statusTiming, ILogger logger)
    {

        _statusTiming = statusTiming;
        _logger = logger;
        _logger.LogInformation("StatusHandler service is starting...");
        _timer = new Timer(PingAllAgent!, null, TimeSpan.Zero, TimeSpan.FromSeconds(statusTiming));
    }

    public void Stop()
    {
        _logger?.LogInformation("StatusHandler service is stopping...");

        _timer?.Change(Timeout.Infinite, 0);
        _timer?.Dispose();
    }

    private void PingAllAgent(object state)
    {
        _logger?.LogInformation($"Pinging all agent at time : {DateTime.Now}");
        _agentRepository.PingAllAgentToTroubleShoothing(_statusTiming != null ? _statusTiming.Value : 60);
    }
}
