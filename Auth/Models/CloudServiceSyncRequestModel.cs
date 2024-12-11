namespace Auth.Models
{
    public class GoogleDriveSyncRequestModel
    {
        public string? Oauth2Token { get; set; }
    }

    public class NotionSyncResponseModel
    {
        public string? DatabaseId { get; set; }
    }
}