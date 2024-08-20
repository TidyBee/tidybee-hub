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
                foreach (var property in page.Properties)
                {
                    var propertyName = property.Key;
                    var propertyValue = property.Value;

                    _logger.LogInformation("Property Name: {PropertyName}", propertyName);
                    _logger.LogInformation("Property Type: {PropertyType}", propertyValue.Type);
                }
            }
        }
    }
}
