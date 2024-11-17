using Microsoft.AspNetCore.SignalR;

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
        var data = _outputService.getTextWidgetUnused(await _inputService.getFiles());
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendGradeWidget()
    {
        var data = _outputService.getGradeWidget(await _inputService.getFiles());
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendTotalMonitored()
    {
        var data = _outputService.getTotalMonitored(await _inputService.getFiles());
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendGraphWidget()
    {
        var data = _outputService.getGraphWidget(await _inputService.getFiles());
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendTextWidgetbadname()
    {
        var data = _outputService.getTextWidgetbadname(await _inputService.getFiles());
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendTextWidgetduplicate()
    {
        var data = _outputService.getTextWidgetduplicate(await _inputService.getFiles());
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendTextWidgetstorage()
    {
        var data = _outputService.getTextWidgetstorage(await _inputService.getFiles());
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendOverviewAll()
    {
        var data = _outputService.getOverviewAll(await _inputService.getFiles());
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendOverviewMisnamed()
    {
        var data = _outputService.getOverviewMisnamed(await _inputService.getFiles(), await _inputService.getRules());
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendOverviewDuplicate()
    {
        var data = _outputService.getOverviewDuplicate(await _inputService.getFiles(), await _inputService.getRules());
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendOverviewUnused()
    {
        var data = _outputService.getOverviewUnused(await _inputService.getFiles(), await _inputService.getRules());
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendTidyRules()
    {
        var data = _outputService.getTidyRules(await _inputService.getRules());
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendFileById(string id)
    {
        var file = await _inputService.GetFileById(id);

        if (file != null)
        {
            await Clients.Caller.SendAsync("ReceiveFileById", file);
        }
        else
        {
            await Clients.Caller.SendAsync("ReceiveError", $"File with ID {id} not found.");
        }
    }
}
