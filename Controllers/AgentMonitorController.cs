using Microsoft.AspNetCore.Mvc;

namespace tidybee_hub.Controllers;

[Route("devtools/[controller]")]
public class AgentMonitorController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
