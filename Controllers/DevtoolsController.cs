using Microsoft.AspNetCore.Mvc;

namespace tidybee_hub.Controllers;

[Route("/[controller]")]
public class DevtoolsController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
