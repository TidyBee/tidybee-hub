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
        await foreach (var update_request in request.ReadAllAsync())
        {
            _logger.LogInformation($"Recieved a request type {update_request.EventType} to update file: {update_request.Path}");

            if (update_request.EventType == FileEventType.Created)
            {
                if (await _context.Files.Where(f => f.Name == update_request.Path[0]).FirstOrDefaultAsync() != null)
                {
                    _logger.LogWarning($"File {update_request.Path} already exists in database");
                    continue;
                }
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
                    Provenance = "agent",
                    DuplicateAssociativeTableDuplicateFiles = [],
                    DuplicateAssociativeTableOriginalFiles = [],
                });
            }
            if (update_request.EventType == FileEventType.Updated)
            {
                var file = await _context.Files.Where(f => f.Name == update_request.Path[0]).FirstOrDefaultAsync();
                if (file != null)
                {
                    file.FileHash = update_request.Hash;
                    file.Size = (int)update_request.Size;
                    file.LastModified = DateTime.Parse(update_request.LastModified.ToString().TrimStart('\"').TrimEnd('\"'));
                    _context.Files.Update(file);
                }
                else
                {
                    _logger.LogWarning($"File {update_request.Path} not found in database");
                }
            }
            if (update_request.EventType == FileEventType.Deleted)
            {
                var file = await _context.Files.Where(f => f.Name == update_request.Path[0]).FirstOrDefaultAsync();
                if (file != null)
                {
                    _context.Files.Remove(file);
                    _context.DuplicateAssociativeTables.RemoveRange(file.DuplicateAssociativeTableDuplicateFiles);
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
            await UpdateScore();
        }

        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
        await UpdateScore();

        return await Task.FromResult(new FileInfoEventResponse
        {
            Status = Status.Ok,
        });
    }

    public override async Task<FileInfoEventResponse> FolderEvent(IAsyncStreamReader<FolderEventRequest> request, ServerCallContext context) {
        await foreach (var update_request in request.ReadAllAsync())
        {
            _logger.LogInformation($"Recieved a request type {update_request.EventType} to update folder: {update_request.OldPath}");

            // Here all the files that are in the folder are deleted
            if (update_request.EventType == FileEventType.Deleted)
            {
                var files = await _context.Files.Where(f => f.Name.StartsWith(update_request.OldPath)).ToListAsync();
                if (files != null)
                {
                    _context.Files.RemoveRange(files);
                    foreach (var file in files)
                    {
                        _context.DuplicateAssociativeTables.RemoveRange(file.DuplicateAssociativeTableDuplicateFiles);
                    }
                }
            }
            // Here all the files that are in the folder are moved to the new folder and thus need to have their paths updated
            if (update_request.EventType == FileEventType.Moved)
            {
                var files = await _context.Files.Where(f => f.Name.StartsWith(update_request.OldPath)).ToListAsync();
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        file.Name = file.Name.Replace(update_request.OldPath, update_request.NewPath);
                    }
                }
            }
        }

        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
        await UpdateScore();

        return await Task.FromResult(new FileInfoEventResponse
        {
            Status = Status.Ok,
        });
    }

    /// <summary>
    /// Update the scores of all the files in the database by calling the stored procedures.
    /// </summary>
    /// <returns></returns>
    private async Task UpdateScore() {
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
}