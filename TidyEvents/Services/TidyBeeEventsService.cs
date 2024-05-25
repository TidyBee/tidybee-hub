using Grpc.Core;
using TidyEvents.Models;
using TidyEvents.Context;
using Microsoft.EntityFrameworkCore;

namespace TidyEvents.Services;

public class TidyBeeEventsService : TidyBeeEvents.TidyBeeEventsBase
{
    private readonly ILogger<TidyBeeEventsService> _logger;

    private readonly DatabaseContext _context;

    public TidyBeeEventsService(ILogger<TidyBeeEventsService> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public override async Task<FileInfoEventResponse> FileEvent(IAsyncStreamReader<FileEventRequest> request, ServerCallContext context)
    {
        // TODO: Buffer events and process them in batches
        await foreach (var update_request in request.ReadAllAsync())
        {
            _logger.LogInformation($"Recieved a request type {update_request.EventType} to update file: {update_request.Path}");

            if (update_request.EventType == FileEventType.Created)
            {
                await _context.Files.AddAsync(new Models.File
                {
                    Name = update_request.Path,
                    FileHash = update_request.Hash,
                    Size = (int)update_request.Size,
                    LastModified = DateTime.Parse(update_request.LastModified.ToString().TrimStart('\"').TrimEnd('\"')),
                    MisnamedScore = 'U',
                    PerishedScore = 'U',
                    DuplicatedScore = 'U',
                    GlobalScore = 'U',
                    DuplicateAssociativeTableDuplicateFiles = new List<DuplicateAssociativeTable>(),
                    DuplicateAssociativeTableOriginalFiles = new List<DuplicateAssociativeTable>(),
                });
            }
            if (update_request.EventType == FileEventType.Deleted)
            {
                var file = await _context.Files.Where(f => f.Name == update_request.Path).FirstOrDefaultAsync();
                if (file != null)
                {
                    _context.Files.Remove(file);
                } else {
                    _logger.LogWarning($"File {update_request.Path} not found in database");
                }
            }

            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }

        return await Task.FromResult(new FileInfoEventResponse
        {
            Status = Status.Ok,
        });
    }
}