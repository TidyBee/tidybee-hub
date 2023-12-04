using AspNetCore.Proxy;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();
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

app.MapControllers();

app.Run();
