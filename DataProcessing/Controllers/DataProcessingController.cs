using Microsoft.AspNetCore.Mvc;

namespace DataProcessing.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataProcessing : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("DataProcessing Service hit");
        }
    }
}