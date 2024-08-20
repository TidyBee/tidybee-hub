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
builder.Services.AddScoped<NotionFileSyncService>();
builder.Services.AddScoped<GoogleDriveSyncService>();

var app = builder.Build();
var scope = app.Services.CreateScope();
var services = scope.ServiceProvider.GetRequiredService<DatabaseContext>().Database.EnsureCreated();

// Configure the HTTP request pipeline.
app.MapGrpcService<TidyBeeEventsService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// Execute the Notion synchronization service
var notionService = scope.ServiceProvider.GetRequiredService<NotionFileSyncService>();
await notionService.SyncFilesFromNotionAsync("secret_OM5WrzIOYfMU3I3T2I39ZxdYe7PBFG647a9eLNGZOt", "0a9d6d2abe854eb399049d12a1e506a2");

var googleDriveService = scope.ServiceProvider.GetRequiredService<GoogleDriveSyncService>();
await googleDriveService.SyncFilesFromGoogleDriveAsync("");

app.Run();