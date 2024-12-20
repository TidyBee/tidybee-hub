using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.EntityFrameworkCore;
using DataProcessing.Context;

var builder = WebApplication.CreateBuilder(args);

var dbConnectionString = builder.Configuration.GetConnectionString("DatabaseConnection");

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(dbConnectionString));

Console.Write(dbConnectionString);

if (WindowsServiceHelpers.IsWindowsService())
{
    builder.Host.UseWindowsService();
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var frontendUrl = builder.Configuration.GetValue<string>("FrontendUrl");

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins(frontendUrl ?? "http://localhost:8080", "http://localhost:8080", "http://prod.tidybee.fr", "https://prod.tidybee.fr")
            .AllowCredentials());
});

builder.Services.AddScoped<OutputService>();
builder.Services.AddScoped<InputService>();

//var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
//builder.Configuration.SetBasePath(AppContext.BaseDirectory)
//                      .AddJsonFile(configPath, optional: false, reloadOnChange: true);

var app = builder.Build();

app.UseCors("CorsPolicy");
var scope = app.Services.CreateScope();
var services = scope.ServiceProvider.GetRequiredService<DatabaseContext>().Database.EnsureCreated();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();
app.MapHub<WidgetHub>("/widgetHub");

app.Run();
