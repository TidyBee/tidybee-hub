using Microsoft.AspNetCore.Mvc;

namespace auth.Controllers;

[Route("[controller]")]
public class AgentMonitorController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
