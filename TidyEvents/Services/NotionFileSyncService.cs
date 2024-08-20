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

                    // Log basic information
                    Console.WriteLine($"Property Name: {propertyName}");

                    // Log property type and value
                    switch (propertyValue.Type)
                    {
                        case PropertyType.Title:
                            foreach (var title in propertyValue.Title)
                            {
                                Console.WriteLine($"Title: {title.PlainText}");
                            }
                            break;

                        case PropertyType.RichText:
                            foreach (var richText in propertyValue.RichText)
                            {
                                Console.WriteLine($"RichText: {richText.PlainText}");
                            }
                            break;

                        case PropertyType.Number:
                            Console.WriteLine($"Number: {propertyValue.Number}");
                            break;

                        case PropertyType.Select:
                            Console.WriteLine($"Select: {propertyValue.Select?.Name}");
                            break;

                        case PropertyType.MultiSelect:
                            Console.WriteLine($"MultiSelect: {string.Join(", ", propertyValue.MultiSelect.Select(s => s.Name))}");
                            break;

                        case PropertyType.Date:
                            Console.WriteLine($"Date: {propertyValue.Date?.Start}");
                            break;

                        case PropertyType.Person:
                            Console.WriteLine($"Person: {string.Join(", ", propertyValue.Person.Select(p => p.Name))}");
                            break;

                        case PropertyType.File:
                            Console.WriteLine($"File: {string.Join(", ", propertyValue.File.Select(f => f.Name))}");
                            break;

                        case PropertyType.Checkbox:
                            Console.WriteLine($"Checkbox: {propertyValue.Checkbox}");
                            break;

                        case PropertyType.Url:
                            Console.WriteLine($"Url: {propertyValue.Url}");
                            break;

                        default:
                            Console.WriteLine($"Unknown Type: {propertyValue.Type}");
                            break;
                    }

                    Console.WriteLine(); // Add an empty line for readability
                }
            }
        }
    }
}
