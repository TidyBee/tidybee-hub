using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Drive.v3.Data;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace TidyEvents.Services
{
    public class GoogleDriveSyncService
    {
        private readonly ILogger<GoogleDriveSyncService> _logger;

        public GoogleDriveSyncService(ILogger<GoogleDriveSyncService> logger)
        {
            _logger = logger;
        }

        public async Task SyncFilesFromGoogleDriveAsync(string apiKey)
        {
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GetCredential(apiKey),
                ApplicationName = "TidyBee"
            });

            _logger.LogInformation($"GOOGLE DRIVE SYNC STARTED");

            var request = service.Files.List();
            request.Fields = "nextPageToken, files(id, name)";
            var result = await request.ExecuteAsync();

            foreach (var file in result.Files)
            {
                Console.WriteLine($"File Name: {file.Name}, File ID: {file.Id}");
            }
        }

        private UserCredential GetCredential(string apiKey)
        {
            // Here you need to configure OAuth2.0 or API Key usage
            // This is a simplified example for OAuth2.0 client credentials
            return GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load("client_secret.json").Secrets,
                new[] { DriveService.Scope.DriveReadonly },
                "user",
                CancellationToken.None
            ).Result;
        }
    }
}
