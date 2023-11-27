using AspNetCore.Proxy;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using tidybee_hub.Context;
using tidybee_hub.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DatabaseConnection")));
builder.Services.AddScoped<AgentRepository>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Configuration.GetValue<bool>("EnableAutoMigration"))
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

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

// app.UseMiddleware<ProxyMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

var AgentURL = app.Configuration.GetValue<Uri>("AgentURL");

app.UseProxies(proxies => {
    proxies.Map("/proxy/{query}", proxy => proxy.UseHttp((context, args) => {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        foreach (var arg in args)
        {
            logger.LogInformation($"Key: {arg.Key}, Value: {arg.Value}");
        }
        string? query = args["query"].ToString();
        return $"{AgentURL}{query}";
    }));
});


app.UseCors(policy => policy
    .AllowAnyOrigin()
       .AllowAnyMethod()
          .AllowAnyHeader());

app.MapControllers();

app.Run();
