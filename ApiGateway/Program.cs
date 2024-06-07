using AspNetCore.Proxy;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Hosting.WindowsServices;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

if (WindowsServiceHelpers.IsWindowsService())
{
    builder.Host.UseWindowsService();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
    policy =>
    {
        policy.WithOrigins(
            "http://localhost:8080"
        );
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
//builder.Configuration.SetBasePath(AppContext.BaseDirectory)
//                      .AddJsonFile(configPath, optional: false, reloadOnChange: true);

var configuration = builder.Configuration;

try
{
    var authServiceUrl = configuration.GetValue<string>("AothServiceUrl") ?? "http://localhost:7002";
    var dataProcessingServiceUrl = configuration.GetValue<string>("DataProcessingServiceUrl") ?? "http://localhost:7003";
    builder.Services.AddHttpClient("AuthServiceClient", client =>
    {
        client.BaseAddress = new Uri(authServiceUrl);
    });

    builder.Services.AddHttpClient("DataProcessingServiceClient", client =>
    {
        client.BaseAddress = new Uri(dataProcessingServiceUrl);
    });
}
catch (Exception ex)
{
    Console.WriteLine("Error loading configuration: " + ex.Message);
    throw;
}


var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);
app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();
app.UseMiddleware<ApiGateway.HiveMiddleware>();

app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();

app.Run();