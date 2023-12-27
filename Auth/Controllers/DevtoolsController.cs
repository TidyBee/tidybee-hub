using Microsoft.AspNetCore.Mvc;

namespace auth.Controllers;

[Route("[controller]")]
public class DevtoolsController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
