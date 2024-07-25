using Grpc.Core;
using TidyEvents.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Runtime.CompilerServices;

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
                    Name = update_request.Path[0],
                    FileHash = update_request.Hash,
                    Size = (int)update_request.Size,
                    LastModified = DateTime.Parse(update_request.LastModified.ToString().TrimStart('\"').TrimEnd('\"')),
                    MisnamedScore = 'U',
                    PerishedScore = 'U',
                    DuplicatedScore = 'U',
                    GlobalScore = 'U',
                    DuplicateAssociativeTableDuplicateFiles = [],
                    DuplicateAssociativeTableOriginalFiles = [],
                });
            }
            if (update_request.EventType == FileEventType.Deleted)
            {
                var file = await _context.Files.Where(f => f.Name == update_request.Path[0]).FirstOrDefaultAsync();
                if (file != null)
                {
                    _context.Files.Remove(file);
                }
                else
                {
                    _logger.LogWarning($"File {update_request.Path} not found in database");
                }
            }
            if (update_request.EventType == FileEventType.Moved) {
                var file = await _context.Files.Where(f => f.Name == update_request.Path[0]).FirstOrDefaultAsync();
                if (file != null)
                {
                    file.Name = update_request.Path[1];
                    file.LastModified = DateTime.Parse(update_request.LastModified.ToString().TrimStart('\"').TrimEnd('\"'));
                }
                else
                {
                    _logger.LogWarning($"File {update_request.Path} not found in database");
                }
            }

            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }


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

        return await Task.FromResult(new FileInfoEventResponse
        {
            Status = Status.Ok,
        });
    }
}