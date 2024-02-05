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

if (app.Configuration.GetValue<bool>("EnableAutoMigration"))
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var dbContext = services.GetRequiredService<DatabaseContext>();
        dbContext.Database.EnsureCreated();
        dbContext.Database.Migrate();
        var agentRepo = services.GetRequiredService<AgentRepository>();
        agentRepo.PingAllAgentToTroubleShoothing();
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

app.Run();