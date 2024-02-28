using System;
using System.Threading;
using auth.Repository;

public class StatusHandlerService
{
    private Timer? timer;
    private AgentRepository? agentRepository;
    private int? statusTiming;

    public void Start(AgentRepository agentRepository, int statusTiming)
    {
        Console.WriteLine("Service is starting...");

        this.agentRepository = agentRepository;
        this.statusTiming = statusTiming;
        timer = new Timer(PrintText, null, TimeSpan.Zero, TimeSpan.FromSeconds(statusTiming));
    }

    public void Stop()
    {
        Console.WriteLine("Service is stopping...");

        timer?.Change(Timeout.Infinite, 0);
        timer?.Dispose();
    }

    private void PrintText(object state)
    {
        Console.WriteLine($"Service is running... {DateTime.Now}");
        this.agentRepository?.PingAllAgentToTroubleShoothing();
    }
}
