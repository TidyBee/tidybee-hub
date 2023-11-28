using Microsoft.AspNetCore.Mvc;

namespace DataProcessingService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataProcessingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("DataProcessing Service hit");
        }
    }
}
