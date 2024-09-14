using Microsoft.Extensions.Logging;
using Notion.Client;
using TidyEvents.Context;
using TidyEvents.Models;
using System.IO.Hashing;
using System.Text;
using Npgsql;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;


namespace TidyEvents.Services
{
    public class NotionFileSyncService
    {
        private readonly ILogger<NotionFileSyncService> _logger;
        private readonly DatabaseContext _context;
        private readonly INotionClient _notionClient;
        private readonly XxHash64 _hasher = new();

        public NotionFileSyncService(ILogger<NotionFileSyncService> logger, DatabaseContext context, INotionClient notionClient)
        {
            _logger = logger;
            _context = context;
            _notionClient = notionClient;
        }

        public async Task SyncFilesFromNotionAsync(string notionDatabaseId)
        {
            _logger.LogInformation($"Retrieving files from Notion");

            var queryResult = await _notionClient.Databases.QueryAsync(notionDatabaseId, new DatabasesQueryParameters
            {
                Filter = null,
            });

            foreach (var page in queryResult.Results)
            {
                var title_property = page.Properties.Where(p => p.Value.Type == PropertyValueType.Title).FirstOrDefault();
                var title = title_property.Value as TitlePropertyValue;

                var last_modified = page.LastEditedTime;

                var page_content = await PageContentToRawText(page);

                var page_size = page_content.Length * sizeof(char);

                var page_contentBytes = Encoding.UTF8.GetBytes(page_content);
                _hasher.Append(page_contentBytes);
                var page_hash = _hasher.GetHashAndReset();

                _logger.LogInformation($"Adding file {title?.Title[0].PlainText}");
  
                await _context.AddAsync(new Models.File
                {
                    Name = title!.Title[0].PlainText,
                    Size = page_size,
                    FileHash = Convert.ToBase64String(page_hash),
                    LastModified = last_modified,
                    MisnamedScore = 'U',
                    PerishedScore = 'U',
                    DuplicatedScore = 'U',
                    GlobalScore = 'U',
                });
            }
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();


            _logger.LogInformation($"Applying stored procedures / Calculating rules");
            var stored_procedures_raw = new List<string>
            {
                "CALL calculate_every_perished_scores();",
                "CALL calculate_every_misnamed_scores();",
                "CALL calculate_every_duplicated_scores();",
                "CALL calculate_every_global_scores();"
            };
            var stored_procedure_fs = stored_procedures_raw.Select(sp_raw => FormattableStringFactory.Create(sp_raw));

            foreach (var sp in stored_procedure_fs)
            {
                try
                {
                    await _context.Database.ExecuteSqlAsync(sp);
                }
                catch (NpgsqlException e)
                {
                    _logger.LogError(e, $"Error executing stored procedure {sp}");
                }
            }
        }
        private async Task<string> PageContentToRawText(Page page) {
            var page_blocks = await _notionClient.Blocks.RetrieveChildrenAsync(page.Id);
            var raw_text = "";

            foreach (var block in page_blocks.Results)
            {
                if (block.Type == BlockType.Paragraph)
                {
                    var paragraph_block = block as ParagraphBlock;
                    raw_text += paragraph_block?.Paragraph.RichText.Aggregate("", (acc, text) => acc + text.PlainText);
                    raw_text = paragraph_block?.Paragraph.RichText.Aggregate(raw_text, (acc, text) => acc + text.PlainText);
                }
                if (block.Type == BlockType.Heading_1) {
                    var heading_block = block as HeadingOneBlock;
                    raw_text += heading_block?.Heading_1.RichText.Aggregate("", (acc, text) => acc + text.PlainText);
                }
                if (block.Type == BlockType.Heading_2) {
                    var heading_block = block as HeadingTwoBlock;
                    raw_text += heading_block?.Heading_2.RichText.Aggregate("", (acc, text) => acc + text.PlainText);
                }
                if (block.Type == BlockType.Heading_3) {
                    var heading_block = block as HeadingThreeBlock;
                    raw_text += heading_block?.Heading_3.RichText.Aggregate("", (acc, text) => acc + text.PlainText);
                }
                if (block.Type == BlockType.BulletedListItem) {
                    var bulleted_list_block = block as BulletedListItemBlock;
                    raw_text += bulleted_list_block?.BulletedListItem.RichText.Aggregate("", (acc, text) => acc + text.PlainText);
                }
                if (block.Type == BlockType.ToDo) {
                    var numbered_list_block = block as ToDoBlock;
                    raw_text += numbered_list_block?.ToDo.RichText.Aggregate("", (acc, text) => acc + text.PlainText);
                }
            }
            // Console.WriteLine($"Paragraphs: {i}");
            return raw_text;
        }
    }

}
