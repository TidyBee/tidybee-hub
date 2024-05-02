using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DataProcessing.Models;

public class OutputService
{
    public OutputService()
    {

    }

    public string GetTextWidgetUnused()
    {
        var data = new
        {
            title = "unused",
            types = "Number",
            data = new
            {
                percentage = "+8",
                value = "407",
                status = false
            }
        };
        var jsonData = JsonConvert.SerializeObject(data);
        return jsonData;
    }
}