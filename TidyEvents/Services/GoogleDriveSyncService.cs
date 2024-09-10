using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Drive.v3;
using System.IO;
using System.Threading.Tasks;
using System.IO.Hashing;
using System.Text;

namespace TidyEvents.Services
{
    public class GoogleDriveSyncService
    {
        private readonly ILogger<GoogleDriveSyncService> _logger;
        private readonly XxHash64 _hasher = new();

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
            request.Fields = "nextPageToken, files(*)";
            var result = await request.ExecuteAsync();

            foreach (var file in result.Files)
            {
                Console.WriteLine($"{file.Name} - {file.Id}");
                var fileStream = new MemoryStream();
                var request2 = service.Files.Get(file.Id);
                await request2.DownloadAsync(fileStream);
                fileStream.Seek(0, SeekOrigin.Begin);
                _hasher.Append(fileStream);
                var fileHash = _hasher.GetHashAndReset();
                Console.WriteLine($"File Name: {file.Name}, File ID: {file.Id}, xxh64: {Convert.ToBase64String(fileHash)}");
            }
        }


        private async Task<GoogleCredential> GetCredentialAsync()
        {
            using var stream = new FileStream("app.json", FileMode.Open, FileAccess.Read);
            var scopes = new[] {
                DriveService.Scope.Drive,
                DriveService.Scope.DriveReadonly,
                DriveService.Scope.DriveMetadataReadonly,
            };
            var credential = GoogleCredential.FromStream(stream)
                .CreateScoped(scopes);

            return credential;
        }
    }
}
