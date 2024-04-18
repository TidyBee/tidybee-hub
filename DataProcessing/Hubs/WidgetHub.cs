using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class WidgetHub : Hub
{
    public async Task SendTextWidgetunused()
    {
        var data = new
        {
            title = "unused",
            types = "Number",
            data = new
            {
                percentage = "+8",
                value = "408",
                status = false
            }
        };
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendGradeWidget()
    {
        var data = new
        {
            grade = "B"
        };
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendGraphWidget()
    {
        var data = new
        {
            series = new[] { 20, 32, 23, 15, 10 }
        };
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendTextWidgetbadname()
    {
        var data = new
        {
            title = "badname",
            types = "Number",
            data = new
            {
                percentage = "-12",
                value = "259",
                status = true
            }
        };
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendTextWidgetduplicate()
    {
        var data = new
        {
            title = "duplicate",
            types = "Number",
            data = new
            {
                percentage = "+19",
                value = "124",
                status = false
            }
        };
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendTextWidgetheavy()
    {
        var data = new
        {
            title = "heavy",
            types = "Number",
            data = new
            {
                percentage = "-5",
                value = "86",
                status = true
            }
        };
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }

    public async Task SendTextWidgetstorage()
    {
        var data = new
        {
            title = "storage",
            types = "Graph",
            data = new
            {
                percentage = "+4",
                value = "237/512GB",
                valuePercentage = "28.32",
                status = false
            }
        };
        await Clients.Caller.SendAsync("ReceiveMessage", data);
    }
}
