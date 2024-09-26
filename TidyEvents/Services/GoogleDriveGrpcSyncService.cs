using Grpc.Core;
using TidyEventsGDrive.Grpc;
using TidyEvents.Services;

public class GoogleDriveGrpcSyncService : GoogleDriveGrpcSync.GoogleDriveGrpcSyncBase
{
    private readonly GoogleDriveSyncService _googleDriveSyncService;

    public GoogleDriveGrpcSyncService(GoogleDriveSyncService googleDriveSyncService)
    {
        _googleDriveSyncService = googleDriveSyncService;
    }

    public override async Task<SyncFilesResponse> SyncFiles(SyncFilesRequest request, ServerCallContext context)
    {
        try
        {
            //await _googleDriveSyncService.SyncFilesFromGoogleDriveAsync(request.Oauth2Token); TODO use when Gdrive remove from the serviceAccount
            await _googleDriveSyncService.SyncFilesFromGoogleDriveAsync();

            return new SyncFilesResponse
            {
                Success = true,
                Message = "Google Drive files synced successfully"
            };
        }
        catch (Exception ex)
        {
            return new SyncFilesResponse
            {
                Success = false,
                Message = $"Failed to sync Google Drive files: {ex.Message}"
            };
        }
    }
}
