using Grpc.Core;
using TidyEvents.Grpc;
using TidyEvents.Services; // Assuming you already have the NotionFileSyncService

public class NotionSyncService : NotionSync.NotionSyncBase
{
    private readonly NotionFileSyncService _notionFileSyncService;

    public NotionSyncService(NotionFileSyncService notionFileSyncService)
    {
        _notionFileSyncService = notionFileSyncService;
    }

    public override async Task<SyncDatabaseResponse> SyncDatabase(SyncDatabaseRequest request, ServerCallContext context)
    {
        try
        {
            await _notionFileSyncService.SyncFilesFromNotionAsync(request.DatabaseId);

            return new SyncDatabaseResponse
            {
                Success = true,
                Message = "Notion database synced successfully"
            };
        }
        catch (Exception ex)
        {
            return new SyncDatabaseResponse
            {
                Success = false,
                Message = $"Failed to sync Notion database: {ex.Message}"
            };
        }
    }
}
