using Microsoft.Extensions.Hosting.WindowsServices;

var builder = WebApplication.CreateBuilder(args);

if (WindowsServiceHelpers.IsWindowsService())
{
    builder.Host.UseWindowsService();
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
builder.Configuration.SetBasePath(AppContext.BaseDirectory)
                      .AddJsonFile(configPath, optional: false, reloadOnChange: true);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
