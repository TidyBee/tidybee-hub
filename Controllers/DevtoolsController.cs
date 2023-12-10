using Microsoft.AspNetCore.Mvc;

namespace tidybee_hub.Controllers;

[Route("/[controller]")]
public class DevtoolsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
