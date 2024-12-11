using Microsoft.EntityFrameworkCore;
using auth.Context;
using auth.Repository;
using Microsoft.Extensions.Hosting.WindowsServices;
using TidyEventsGDrive.Grpc;
using TidyEventsNotion.Grpc;

var builder = WebApplication.CreateBuilder(args);

if (WindowsServiceHelpers.IsWindowsService())
{
    builder.Host.UseWindowsService();
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DatabaseConnection")));
builder.Services.AddScoped<AgentRepository>();
builder.Services.AddScoped<StatusHandlerService>();

builder.Services.AddGrpcClient<GoogleDriveGrpcSync.GoogleDriveGrpcSyncClient>(options =>
{
    var tidyevents_address = builder.Configuration.GetValue<string>("TidyEvents:Address");
    var tidyevents_port = builder.Configuration.GetValue<string>("TidyEvents:Port");

    options.Address = new Uri($"https://{tidyevents_address}:{tidyevents_port}");
});

builder.Services.AddGrpcClient<NotionSync.NotionSyncClient>(options =>
{
    var tidyevents_address = builder.Configuration.GetValue<string>("TidyEvents:Address");
    var tidyevents_port = builder.Configuration.GetValue<string>("TidyEvents:Port");

    options.Address = new Uri($"https://{tidyevents_address}:{tidyevents_port}");
});


builder.Services.AddCors(options =>
{   
    options.AddPolicy("CorsPolicy",
        builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("http://localhost:8080", "https://prod.tidybee.fr:8080", "https://prod.tidybee.fr")
            .AllowCredentials());
});
//var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
//builder.Configuration.SetBasePath(AppContext.BaseDirectory)
//                      .AddJsonFile(configPath, optional: false, reloadOnChange: true);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var dbContext = services.GetRequiredService<DatabaseContext>();
dbContext.Database.EnsureCreated();

var logger = services.GetRequiredService<ILogger<Program>>();

if (app.Configuration.GetValue<bool>("EnableAutoMigration"))
{
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

StatusHandlerService? statusHandler = null;
int statusHandlerFrequency = app.Configuration.GetValue<int>("StatusHandlerFrequency");

if (statusHandlerFrequency != 0)
{
    try
    {
        statusHandler = services.GetRequiredService<StatusHandlerService>();
        statusHandler.Start(statusHandlerFrequency, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Unable to setup the StatusHandler service.");
    }
}

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

statusHandler.Stop();