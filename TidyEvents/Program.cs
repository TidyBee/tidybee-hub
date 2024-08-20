using Microsoft.EntityFrameworkCore;
using TidyEvents.Context;
using TidyEvents;
using TidyEvents.Interceptors;
using TidyEvents.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection")));
builder.Services.AddGrpc().AddServiceOptions<TidyBeeEventsService>(options =>
{
    options.Interceptors.Add<AuthInterceptor>();
});
builder.Services.Configure<AuthInterceptorOption>(builder.Configuration.GetSection("AuthInterceptor"));
builder.Services.AddHttpClient<AuthInterceptor>();

var app = builder.Build();
var scope = app.Services.CreateScope();
var services = scope.ServiceProvider.GetRequiredService<DatabaseContext>().Database.EnsureCreated();

// Configure the HTTP request pipeline.
app.MapGrpcService<TidyBeeEventsService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
var notionService = new NotionFileSyncService(logger, dbContext, grpcClient);
await notionService.SyncFilesFromNotionAsync("secret_5BnuviYZnZuHc6Ji1vUv8M0PcRayV9SxnWl0uVwvRIH", "840184a42f0a41ff864ae5533d30e670");
app.Run();
