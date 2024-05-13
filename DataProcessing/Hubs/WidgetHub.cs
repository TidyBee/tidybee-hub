using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class WidgetHub : Hub
{
    private readonly OutputService _outputService;
    private readonly InputService _inputService;
    public WidgetHub(OutputService outputService, InputService inputService)
    {
        _outputService = outputService;
        _inputService = inputService;
    }

    public async Task SendTextWidgetunused()
    {
        var data = _outputService.getTextWidgetUnused();
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendGradeWidget()
    {
        var data = _outputService.getGradeWidget();
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendTotalMonitored()
    {
        var data = _outputService.getTotalMonitored();
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendGraphWidget()
    {
        var data = _outputService.getGraphWidget();
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendTextWidgetbadname()
    {
        var data = _outputService.getTextWidgetbadname();
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendTextWidgetduplicate()
    {
        var data = _outputService.getTextWidgetduplicate();
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendTextWidgetstorage()
    {
        var data = _outputService.getTextWidgetstorage();
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendOverviewAll()
    {
        var data = _outputService.getOverviewAll();
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendOverviewMisnamed()
    {
        var data = _outputService.getOverviewMisnamed();
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendOverviewDuplicate()
    {
        var data = _outputService.getOverviewDuplicate();
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendOverviewUnused()
    {
        var data = _outputService.getOverviewUnused();
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendTidyRules()
    {
        var data = _inputService.getRules();
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }
}
