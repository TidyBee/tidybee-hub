using Microsoft.AspNetCore.Mvc;

namespace UserAuth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserAuth : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("UserAuth Service hit");
        }
    }
}