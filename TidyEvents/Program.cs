using Microsoft.EntityFrameworkCore;
using TidyEvents.Context;
using TidyEvents.Interceptors;
using TidyEvents.Services;


var builder = WebApplication.CreateBuilder(args);

// Temporary fix for Npgsql timestamp "german from 44" behavior
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection")));
builder.Services.AddGrpc().AddServiceOptions<TidyBeeEventsService>(options =>
{
    options.Interceptors.Add<AuthInterceptor>();
});
builder.Services.Configure<AuthInterceptorOption>(builder.Configuration.GetSection("AuthInterceptor"));
builder.Services.AddHttpClient<AuthInterceptor>();

builder.Services.AddNotionClient(options => {
    options.AuthToken = builder.Configuration.GetSection("Notion:AuthToken").Value;
});

builder.Services.AddScoped<NotionFileSyncService>();
builder.Services.AddScoped<GoogleDriveSyncService>();


var app = builder.Build();
var scope = app.Services.CreateScope();
var services = scope.ServiceProvider.GetRequiredService<DatabaseContext>().Database.EnsureCreated();

// Configure the HTTP request pipeline.
app.MapGrpcService<TidyBeeEventsService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

await app.RunAsync();