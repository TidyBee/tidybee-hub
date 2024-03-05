using Microsoft.EntityFrameworkCore;
using auth.Context;
using auth.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DatabaseConnection")));
builder.Services.AddScoped<AgentRepository>();
builder.Services.AddScoped<StatusHandlerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

statusHandler?.Stop();