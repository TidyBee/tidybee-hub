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

        public async Task SyncFilesFromGoogleDriveAsync(string accessToken)
        {
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GetCredentialAsync(accessToken),
                ApplicationName = "YourAppName"
            });

            _logger.LogInformation($"Retrieving user drives from Google Drive");

            var drive_request = service.Drives.List();
            drive_request.Fields = "drives(id,name)";

            var drives = await drive_request.ExecuteAsync();

            _logger.LogInformation($"Found {drives.Drives.Count} drives");
            _logger.LogInformation($"Retrieving files from {drives.Drives[0].Name} (drive id: {drives.Drives[0].Id})");

            var request = service.Files.List();
            request.Corpora = "drive";
            request.DriveId = drives.Drives[0].Id;
            request.SupportsAllDrives = true;
            request.IncludeItemsFromAllDrives = true;
            request.Fields = $"nextPageToken,files(*)";

            var result = await request.ExecuteAsync();
            List<string> files_present_in_drive = new();

            foreach (var file in result.Files)
            {
                string fileHash;

                // Skip if it's a folder
                if (file.MimeType == "application/vnd.google-apps.folder")
                {
                    continue;
                }
                fileHash = file.Sha256Checksum ?? file.Sha1Checksum ?? file.Md5Checksum ?? await GetFileHashAsync(service, file.Id);

                var existingFile = await _context.Files.FirstOrDefaultAsync(f => f.Name == file.Name);

                if (existingFile != null)
                {
                    _logger.LogInformation($"Updating file: {file.Name}");

                    existingFile.Size = (int)file.Size!;
                    existingFile.FileHash = fileHash;
                    existingFile.LastModified = DateTime.Parse(file.ModifiedTimeRaw);
                } else {
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
                    files_present_in_drive.Add(file.Name);
            }
            _logger.LogInformation($"Looking for deleted files");
            var files_to_delete = await _context.Files.Where(f => !files_present_in_drive.Contains(f.Name)).ToListAsync();
            _logger.LogInformation($"Found {files_to_delete.Count} files to delete");
            _context.RemoveRange(files_to_delete);

            _logger.LogInformation($"Saving changes to database");
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

        private GoogleCredential GetCredentialAsync(string accessToken)
        {
            // Commenting this part because we might need to keep it to use a service account in the future
            //
            // var service_account_keys_path = _configuration.GetSection("GoogleDrive:ServiceAccountKeysPath").Value;
            // using var stream = new FileStream(service_account_keys_path!, FileMode.Open, FileAccess.Read);
            // var scopes = new[] {
            //     DriveService.Scope.Drive,
            //     DriveService.Scope.DriveReadonly,
            //     DriveService.Scope.DriveMetadataReadonly,
            // };
            // var credential = GoogleCredential.FromStream(stream)
            //     .CreateScoped(scopes);
            var credential = GoogleCredential.FromAccessToken(accessToken);

            return credential;
        }
    }
}
