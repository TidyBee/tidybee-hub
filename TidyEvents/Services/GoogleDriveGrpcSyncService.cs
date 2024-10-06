using Grpc.Core;
using TidyEventsGDrive.Grpc;
using TidyEvents.Services;

public class GoogleDriveGrpcSyncService : GoogleDriveGrpcSync.GoogleDriveGrpcSyncBase
{
    private readonly GoogleDriveSyncService _googleDriveSyncService;
    private readonly ILogger<GoogleDriveGrpcSyncService> _logger;

    public GoogleDriveGrpcSyncService(GoogleDriveSyncService googleDriveSyncService, ILogger<GoogleDriveGrpcSyncService> logger)
    {
        _googleDriveSyncService = googleDriveSyncService;
        _logger = logger;
    }

    public override async Task<SyncFilesResponse> SyncFiles(SyncFilesRequest request, ServerCallContext context)
    {
        try
        {
            //await _googleDriveSyncService.SyncFilesFromGoogleDriveAsync(request.Oauth2Token); TODO use when Gdrive remove from the serviceAccount
            await _googleDriveSyncService.SyncFilesFromGoogleDriveAsync(request.Oauth2Token);

            return new SyncFilesResponse
            {
                Success = true,
                Message = "Google Drive files synced successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync Google Drive files");
            return new SyncFilesResponse
            {
                Success = false,
                Message = $"Failed to sync Google Drive files: {ex.Message}"
            };
        }
    }
}
