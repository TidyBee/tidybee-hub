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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

if (app.Configuration.GetValue<bool>("EnableAutoMigration"))
{
    try
    {
        var dbContext = services.GetRequiredService<DatabaseContext>();
        dbContext.Database.EnsureCreated();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

var agentRepo = services.GetRequiredService<AgentRepository>();
var statusHandler = new StatusHandlerService();
statusHandler.Start(agentRepo, 20);

app.Run();

statusHandler.Stop();