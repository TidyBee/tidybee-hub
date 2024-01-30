using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[Route("[controller]")]
public class DevtoolsController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
