using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Drive.v3;
using System.IO;
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


        public async Task SyncFilesFromGoogleDriveAsync()
        {
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = await GetCredentialAsync(),
                ApplicationName = "YourAppName"
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

        private async Task<GoogleCredential> GetCredentialAsync()
        {
            using var stream = new FileStream("/app/app.json", FileMode.Open, FileAccess.Read);
            var credential = GoogleCredential.FromStream(stream)
                .CreateScoped(DriveService.Scope.DriveReadonly);

            return credential;
        }
    }
}
