using Microsoft.EntityFrameworkCore;
using DataProcessing.Context;
using DataProcessing;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OutputService>();
builder.Services.AddScoped<InputService>();

Console.WriteLine(builder.Configuration.GetConnectionString("DatabaseConnection"));
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql("Host=hub-postgres;Database=db;Username=user;Password=pass");

var app = builder.Build();
var scope = app.Services.CreateScope();
var services = scope.ServiceProvider.GetRequiredService<DatabaseContext>().Database.EnsureCreated();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
