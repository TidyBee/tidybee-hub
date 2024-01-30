using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public async Task<IActionResult> AuthGet(string path)
        {
            return await SendAuthRequest(HttpMethod.Get, path, null);
        }

        [HttpPost("auth/{*path}")]
        public async Task<IActionResult> AuthPost(string path, [FromBody] object requestBody)
        {
            return await SendAuthRequest(HttpMethod.Post, path, requestBody);
        }

        [HttpPut("auth/{*path}")]
        public async Task<IActionResult> AuthPut(string path, [FromBody] object requestBody)
        {
            return await SendAuthRequest(HttpMethod.Put, path, requestBody);
        }

        [HttpDelete("auth/{*path}")]
        public async Task<IActionResult> AuthDelete(string path)
        {
            return await SendAuthRequest(HttpMethod.Delete, path, null);
        }

        private async Task<IActionResult> SendAuthRequest(HttpMethod httpMethod, string path, object? requestBody)
        {
            var client = _clientFactory.CreateClient("AuthServiceClient");
            var requestPath = $"/{path}";

            HttpRequestMessage request;

            if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put)
            {
                var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json"));
                request = new HttpRequestMessage(httpMethod, requestPath) { Content = content };
            }
            else
            {
                request = new HttpRequestMessage(httpMethod, requestPath);
            }

            var response = await client.SendAsync(request);

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
        public async Task<IActionResult> DataProcessingGet(string path)
        {
            return await SendDataProcessingRequest(HttpMethod.Get, path, null);
        }

        [HttpPost("dataProcessing/{*path}")]
        public async Task<IActionResult> DataProcessingPost(string path, [FromBody] object requestBody)
        {
            return await SendDataProcessingRequest(HttpMethod.Post, path, requestBody);
        }

        [HttpPut("dataProcessing/{*path}")]
        public async Task<IActionResult> DataProcessingPut(string path, [FromBody] object requestBody)
        {
            return await SendDataProcessingRequest(HttpMethod.Put, path, requestBody);
        }

        [HttpDelete("dataProcessing/{*path}")]
        public async Task<IActionResult> DataProcessingDelete(string path)
        {
            return await SendDataProcessingRequest(HttpMethod.Delete, path, null);
        }

        private async Task<IActionResult> SendDataProcessingRequest(HttpMethod httpMethod, string path, object? requestBody)
        {
            var client = _clientFactory.CreateClient("DataProcessingServiceClient");
            var requestPath = $"/{path}";

            HttpRequestMessage request;

            if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put)
            {
                var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json"));
                request = new HttpRequestMessage(httpMethod, requestPath) { Content = content };
            }
            else
            {
                request = new HttpRequestMessage(httpMethod, requestPath);
            }

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return Ok(responseContent);
            }

            return StatusCode((int)response.StatusCode, "Error calling Data Processing service");
        }

        [HttpGet("other")]
        public IActionResult Other()
        {
            return Ok("Other route hit");
        }
    }
}
