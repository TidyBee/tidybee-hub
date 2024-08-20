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

                    // Log property name
                    Console.WriteLine($"Property Name: {propertyName}");

                    // Check property type and log value
                    if (propertyValue is TitleProperty titleProperty)
                    {
                        foreach (var title in titleProperty.Title)
                        {
                            Console.WriteLine($"Title: {title.PlainText}");
                        }
                    }
                    else if (propertyValue is RichTextProperty richTextProperty)
                    {
                        foreach (var richText in richTextProperty.RichText)
                        {
                            Console.WriteLine($"RichText: {richText.PlainText}");
                        }
                    }
                    else if (propertyValue is NumberProperty numberProperty)
                    {
                        Console.WriteLine($"Number: {numberProperty.Number}");
                    }
                    else if (propertyValue is SelectProperty selectProperty)
                    {
                        Console.WriteLine($"Select: {selectProperty.Select?.Name}");
                    }
                    else if (propertyValue is MultiSelectProperty multiSelectProperty)
                    {
                        Console.WriteLine($"MultiSelect: {string.Join(", ", multiSelectProperty.MultiSelect.Select(s => s.Name))}");
                    }
                    else if (propertyValue is DateProperty dateProperty)
                    {
                        Console.WriteLine($"Date: {dateProperty.Date?.Start}");
                    }
                    else if (propertyValue is PersonProperty personProperty)
                    {
                        Console.WriteLine($"Person: {string.Join(", ", personProperty.Person.Select(p => p.Name))}");
                    }
                    else if (propertyValue is FileProperty fileProperty)
                    {
                        Console.WriteLine($"File: {string.Join(", ", fileProperty.File.Select(f => f.Name))}");
                    }
                    else if (propertyValue is CheckboxProperty checkboxProperty)
                    {
                        Console.WriteLine($"Checkbox: {checkboxProperty.Checkbox}");
                    }
                    else if (propertyValue is UrlProperty urlProperty)
                    {
                        Console.WriteLine($"Url: {urlProperty.Url}");
                    }
                    else
                    {
                        Console.WriteLine("Unknown Property Type");
                    }

                    Console.WriteLine(); // Add an empty line for readability
                }
            }
        }
    }
}
