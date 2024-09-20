using Microsoft.AspNetCore.Mvc;
using TidyEvents.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TidyEvents.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly NotionFileSyncService _notionFileSyncService;
        private readonly GoogleDriveSyncService _googleDriveSyncService;
        private readonly ILogger<SyncController> _logger;

        public SyncController(NotionFileSyncService notionFileSyncService, GoogleDriveSyncService googleDriveSyncService, ILogger<SyncController> logger)
        {
            _notionFileSyncService = notionFileSyncService;
            _googleDriveSyncService = googleDriveSyncService;
            _logger = logger;
        }

        [HttpPost("notion/database")]
        public async Task<IActionResult> SyncNotionDatabase([FromBody] NotionSyncRequest request)
        {
            if (string.IsNullOrEmpty(request.DatabaseId))
            {
                return BadRequest(new { message = "DatabaseId is required." });
            }

            await _notionFileSyncService.SyncFilesFromNotionAsync(request.DatabaseId);
            return Ok(new { message = $"Notion database {request.DatabaseId} synchronized successfully." });
        }

        [HttpPost("gdrive/token")]
        public async Task<IActionResult> SyncGoogleDriveToken([FromBody] GoogleDriveSyncRequest request)
        {
            if (string.IsNullOrEmpty(request.OAuth2Token))
            {
                return BadRequest(new { message = "OAuth2 token is required." });
            }

            // Log the token (for testing purposes)
            _logger.LogInformation("Google Drive OAuth2 token: {Token}", request.OAuth2Token);

            // Here you can use the OAuth2 token if necessary
            await _googleDriveSyncService.SyncFilesFromGoogleDriveAsync();

            return Ok(new { message = "Google Drive token logged successfully." });
        }
    }

    // DTOs for requests
    public class NotionSyncRequest
    {
        public string DatabaseId { get; set; }
    }

    public class GoogleDriveSyncRequest
    {
        public string OAuth2Token { get; set; }
    }
}
