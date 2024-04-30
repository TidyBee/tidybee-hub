using Grpc.Core;
using Microsoft.AspNetCore.Authentication;

namespace TidyEvents.Services;

public class TidyBeeEventsService : TidyBeeEvents.TidyBeeEventsBase
{
    private readonly ILogger<TidyBeeEventsService> _logger;

    public TidyBeeEventsService(ILogger<TidyBeeEventsService> logger)
    {
        _logger = logger;
    }

    public override async Task<FileInfoEventResponse> FileEvent(IAsyncStreamReader<FileEventRequest> request, ServerCallContext context)
    {
        await foreach (var update_request in request.ReadAllAsync())
        {
            _logger.LogInformation($"Recieved a request to update file: {update_request}");
        }

        return await Task.FromResult(new FileInfoEventResponse
        {
            Status = Status.Ok,
        });
    }
}