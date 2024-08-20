using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Notion.Client;
using TidyEvents.Context;
using TidyEvents.Models;

namespace TidyEvents.Services
{
    public class NotionFileSyncService
    {
        private readonly ILogger<NotionFileSyncService> _logger;
        private readonly DatabaseContext _context;
        private readonly TidyBeeEvents.TidyBeeEventsClient _grpcClient;

        public NotionFileSyncService(ILogger<NotionFileSyncService> logger, DatabaseContext context, TidyBeeEvents.TidyBeeEventsClient grpcClient)
        {
            _logger = logger;
            _context = context;
            _grpcClient = grpcClient;
        }

        public async Task SyncFilesFromNotionAsync(string notionApiToken, string notionDatabaseId)
        {
            var client = NotionClientFactory.Create(new ClientOptions
            {
                AuthToken = notionApiToken
            });

            var database = await client.Databases.RetrieveAsync(notionDatabaseId);
            var queryResult = await client.Databases.QueryAsync(notionDatabaseId);

            foreach (var page in queryResult.Results)
            {
                var fileName = page.Properties["Name"].Title[0].PlainText;
                var fileHash = page.Properties["Hash"].RichText[0].PlainText;
                var fileSize = int.Parse(page.Properties["Size"].Number.ToString());
                var fileLastModified = DateTime.Parse(page.Properties["Last Modified"].LastEditedTime);

                var fileExists = await _context.Files.AnyAsync(f => f.Name == fileName);
                if (!fileExists)
                {
                    var fileEventRequest = new FileEventRequest
                    {
                        EventType = FileEventType.Created,
                        Path = { fileName },
                        Hash = fileHash,
                        Size = (ulong)fileSize,
                        LastModified = fileLastModified.ToString("o") // Format to ISO 8601
                    };

                    using var call = _grpcClient.FileEvent();
                    await call.RequestStream.WriteAsync(fileEventRequest);

                    await call.RequestStream.CompleteAsync();
                    var response = await call.ResponseAsync;

                    if (response.Status == Status.Ok)
                    {
                        _logger.LogInformation($"Successfully synced file: {fileName} from Notion");
                    }
                    else
                    {
                        _logger.LogError($"Failed to sync file: {fileName} from Notion");
                    }
                }
                else
                {
                    _logger.LogInformation($"File {fileName} already exists in the database. Skipping.");
                }
            }
        }
    }
}
