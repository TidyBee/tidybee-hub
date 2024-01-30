using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[Route("devtools/[controller]")]
public class AgentMonitorController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
