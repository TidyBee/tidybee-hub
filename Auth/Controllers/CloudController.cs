using Microsoft.AspNetCore.Mvc;
using TidyEventsGDrive.Grpc;
using TidyEventsNotion.Grpc;
using Auth.Models;

namespace auth.Controllers;

[ApiController]
[Route("[controller]")]
public class CloudController : ControllerBase
{
    private readonly GoogleDriveGrpcSync.GoogleDriveGrpcSyncClient _googleDriveGrpcSyncClient;
    private readonly NotionSync.NotionSyncClient _notionGrpcSyncClient;

    public CloudController(GoogleDriveGrpcSync.GoogleDriveGrpcSyncClient googleDriveGrpcSyncClient, NotionSync.NotionSyncClient notionGrpcSyncClient)
    {
        _googleDriveGrpcSyncClient = googleDriveGrpcSyncClient;
        _notionGrpcSyncClient = notionGrpcSyncClient;
    }

    [HttpPost("SyncGoogle")]
    public async Task<IActionResult> SyncGoogle([FromBody] GoogleDriveSyncRequestModel googleDriveSyncRequestModel)
    {
        if (googleDriveSyncRequestModel.Oauth2Token == null)
        {
            return BadRequest("Oauth2Token is required");
        }

        var sync_request = await _googleDriveGrpcSyncClient.SyncFilesAsync(new SyncFilesRequest
        {
            Oauth2Token = googleDriveSyncRequestModel.Oauth2Token
        });

        if (!sync_request.Success)
        {
            return BadRequest($"Google Drive files sync failed with error: {sync_request.Message}");
        }
        
        return Ok("Google Drive files synced successfully");
    }

    [HttpPost("SyncNotion")]
    public async Task<IActionResult> SyncNotion([FromBody] NotionSyncResponseModel notionSyncResponseModel)
    {
        if (notionSyncResponseModel.DatabaseId == null)
        {
            return BadRequest("DatabaseId is required");
        }

        var sync_request = await _notionGrpcSyncClient.SyncDatabaseAsync(new SyncDatabaseRequest
        {
            DatabaseId = notionSyncResponseModel.DatabaseId
        });

        if (!sync_request.Success)
        {
            return BadRequest($"Notion database sync failed with error: {sync_request.Message}");
        }

        return Ok("Notion database synced successfully");
    }
}