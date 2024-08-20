using Microsoft.Extensions.Logging;
using Notion.Net;
using TidyEvents.Context;
using TidyEvents.Models;

namespace TidyEvents.Services
{
    public class NotionFileSyncService
    {
        private readonly ILogger<NotionFileSyncService> _logger;
        private readonly DatabaseContext _context;

        public NotionFileSyncService(ILogger<NotionFileSyncService> logger, DatabaseContext context)
        {
            _logger = logger;
            _context = context;
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
                _logger.LogInformation($"Successfully synced file: {fileName} from Notion");
            }
        }
    }
}
