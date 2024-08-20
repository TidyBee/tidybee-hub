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

            _logger.LogInformation($"NOTION STARTED");

            var database = await client.Databases.RetrieveAsync(notionDatabaseId);

            var queryResult = await client.Databases.QueryAsync(notionDatabaseId, new DatabasesQueryParameters
            {
                Filter = null
            });

            foreach (var page in queryResult.Results)
            {
                _logger.LogInformation($"Successfully synced file: {page.Properties} from Notion");
            }
        }
    }
}
