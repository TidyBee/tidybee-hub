using Microsoft.AspNetCore.Mvc;

namespace tidybee_hub.Controllers;

[Route("devtools/[controller]")]
public class AgentMonitorController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
