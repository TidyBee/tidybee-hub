using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayController : ControllerBase
    {
        [HttpGet("auth")]
        public IActionResult Auth()
        {
            return Ok("Auth route hit");
        }

        [HttpGet("other")]
        public IActionResult Other()
        {
            return Ok("Other route hit");
        }
    }
}
