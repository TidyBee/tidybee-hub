using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public GatewayController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet("auth/{*path}")]
        public async Task<IActionResult> Auth(string path)
        {
            var client = _clientFactory.CreateClient("AuthServiceClient");
            var requestPath = $"/{path}";
            var response = await client.GetAsync(requestPath);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return Ok(responseContent);
            }

            return StatusCode((int)response.StatusCode, "Error calling Auth service");
        }

        [HttpGet("auth/Agent/test")]
        public async Task<IActionResult> AuthAgentTest()
        {
            var client = _clientFactory.CreateClient("AuthServiceClient");
            var response = await client.GetAsync("/Agent/test");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return Ok(responseContent);
            }

            return StatusCode((int)response.StatusCode, "Error calling Auth service");
        }

        [HttpGet("dataProcessing/{*path}")]
        public async Task<IActionResult> DataProcessing(string path)
        {
            var client = _clientFactory.CreateClient("DataProcessingServiceClient");
            var requestPath = $"/{path}";
            var response = await client.GetAsync(requestPath);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return Ok(responseContent);
            }

            return StatusCode((int)response.StatusCode, "Error calling Auth service");
        }

        [HttpGet("other")]
        public IActionResult Other()
        {
            return Ok("Other route hit");
        }
    }
}
