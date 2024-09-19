using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Drive.v3;
using System.IO.Hashing;
using TidyEvents.Models;
using TidyEvents.Context;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace TidyEvents.Services
{
    public class GoogleDriveSyncService
    {
        private readonly ILogger<GoogleDriveSyncService> _logger;

        private readonly XxHash64 _hasher = new();
        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;

        public GoogleDriveSyncService(ILogger<GoogleDriveSyncService> logger, DatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SyncFilesFromGoogleDriveAsync()
        {
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = await GetCredentialAsync(),
                ApplicationName = "YourAppName"
            });

            _logger.LogInformation($"Retrieving files from Google Drive");

            var request = service.Files.List();
            request.Fields = "nextPageToken, files(*)";
          
            var result = await request.ExecuteAsync();

            foreach (var file in result.Files)
            {
                string fileHash;

                // Skip if it's a folder
                if (file.MimeType == "application/vnd.google-apps.folder")
                {
                    continue;
                }

                fileHash = file.Sha256Checksum ?? file.Sha1Checksum ?? file.Md5Checksum ?? await GetFileHashAsync(service, file.Id);

                _logger.LogInformation($"New file: {file.Name}");

                _context.Add(new Models.File
                {
                    Name = file.Name,
                    Size = (int)file.Size!,
                    FileHash = fileHash,
                    LastModified = DateTime.Parse(file.ModifiedTimeRaw),
                    MisnamedScore = 'U',
                    PerishedScore = 'U',
                    DuplicatedScore = 'U',
                    GlobalScore = 'U',
                });
            }
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var stored_procedures_raw = new List<string>
            {
                "CALL calculate_every_perished_scores();",
                "CALL calculate_every_misnamed_scores();",
                "CALL calculate_every_duplicated_scores();",
                "CALL calculate_every_global_scores();"
            };
            var stored_procedure_fs = stored_procedures_raw.Select(sp_raw => FormattableStringFactory.Create(sp_raw));

            _logger.LogInformation($"Applying stored procedures / Calculating rules");
            foreach (var sp in stored_procedure_fs)
            {
                try
                {
                    await _context.Database.ExecuteSqlAsync(sp);
                }
                catch (Npgsql.NpgsqlException e)
                {
                    _logger.LogError(e, $"Error executing stored procedure {sp}");
                }
            }
        }

        private async Task<string> GetFileHashAsync(DriveService service, string fileId)
        {
            var fileStream = new MemoryStream();
            var request = service.Files.Get(fileId);
            await request.DownloadAsync(fileStream);
            fileStream.Seek(0, SeekOrigin.Begin);
            _hasher.Append(fileStream);
            var fileHash = _hasher.GetHashAndReset();
            return Convert.ToBase64String(fileHash);
        }

        private async Task<GoogleCredential> GetCredentialAsync()
        {
            var service_account_keys_path = _configuration.GetSection("GoogleDrive:ServiceAccountKeysPath").Value;
            using var stream = new FileStream(service_account_keys_path!, FileMode.Open, FileAccess.Read);
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
