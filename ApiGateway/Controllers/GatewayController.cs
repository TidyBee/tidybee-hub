using System.Text;
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
        public async Task<IActionResult> AuthGet(string path, [FromQuery] Dictionary<string, string> queryParams)
        {
            return await SendAuthRequest(HttpMethod.Get, path, null, queryParams, Request.Host);
        }


        [HttpPost("auth/{*path}")]
        public async Task<IActionResult> AuthPost(string path, [FromBody] object requestBody, [FromQuery] Dictionary<string, string> queryParams)
        {
            return await SendAuthRequest(HttpMethod.Post, path, requestBody.ToString(), queryParams, Request.Host);
        }

        [HttpPut("auth/{*path}")]
        public async Task<IActionResult> AuthPut(string path, [FromBody] object requestBody, [FromQuery] Dictionary<string, string> queryParams)
        {
            return await SendAuthRequest(HttpMethod.Put, path, requestBody.ToString(), queryParams, Request.Host);
        }

        [HttpDelete("auth/{*path}")]
        public async Task<IActionResult> AuthDelete(string path, [FromQuery] Dictionary<string, string> queryParams)
        {
            return await SendAuthRequest(HttpMethod.Delete, path, null, queryParams, Request.Host);
        }

        private async Task<IActionResult> SendAuthRequest(HttpMethod httpMethod, string path, string? requestBody, Dictionary<string, string> queryParams, HostString host)
        {
            var client = _clientFactory.CreateClient("AuthServiceClient");
            var requestPath = $"/{path}";

            client.DefaultRequestHeaders.Add("Host", $"{host.Host}:{host.Port}");

            if (queryParams != null && queryParams.Count > 0)
            {
                var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                requestPath += $"?{queryString}";
            }

            HttpRequestMessage request;
            if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put)
            {
                request = new HttpRequestMessage(httpMethod, requestPath)
                {
                    Content = new StringContent(requestBody!, Encoding.UTF8, "application/json")
                };
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
        public async Task<IActionResult> DataProcessingGet(string path, [FromQuery] Dictionary<string, string> queryParams)
        {
            return await SendDataProcessingRequest(HttpMethod.Get, path, null, queryParams);
        }

        [HttpPost("dataProcessing/{*path}")]
        public async Task<IActionResult> DataProcessingPost(string path, [FromBody] object requestBody, [FromQuery] Dictionary<string, string> queryParams)
        {
            return await SendDataProcessingRequest(HttpMethod.Post, path, requestBody.ToString(), queryParams);
        }

        [HttpPut("dataProcessing/{*path}")]
        public async Task<IActionResult> DataProcessingPut(string path, [FromBody] object requestBody, [FromQuery] Dictionary<string, string> queryParams)
        {
            return await SendDataProcessingRequest(HttpMethod.Put, path, requestBody.ToString(), queryParams);
        }

        [HttpDelete("dataProcessing/{*path}")]
        public async Task<IActionResult> DataProcessingDelete(string path, [FromQuery] Dictionary<string, string> queryParams)
        {
            return await SendDataProcessingRequest(HttpMethod.Delete, path, null, queryParams);
        }

        private async Task<IActionResult> SendDataProcessingRequest(HttpMethod httpMethod, string path, string? requestBody, Dictionary<string, string> queryParams)
        {
            var client = _clientFactory.CreateClient("DataProcessingServiceClient");
            var requestPath = $"/{path}";

            if (queryParams != null && queryParams.Count > 0)
            {
                var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                requestPath += $"?{queryString}";
            }

            HttpRequestMessage request;

            if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put)
            {
                request = new HttpRequestMessage(httpMethod, requestPath)
                {
                    Content = new StringContent(requestBody!, Encoding.UTF8, "application/json")
                };
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
