using Grpc.Core;
using TidyEventsGDrive.Grpc; // Ensure this matches the namespace from the generated code
using TidyEvents.Services; // Assuming you have the GoogleDriveSyncService here

public class GoogleDriveSyncService : GoogleDriveSync.GoogleDriveSyncBase
{
    private readonly GoogleDriveSyncService _googleDriveSyncService;

    public GoogleDriveSyncService(GoogleDriveSyncService googleDriveSyncService)
    {
        _googleDriveSyncService = googleDriveSyncService;
    }

    public override async Task<SyncFilesResponse> SyncFiles(SyncFilesRequest request, ServerCallContext context)
    {
        try
        {
            // Call your existing service logic to sync files from Google Drive
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
