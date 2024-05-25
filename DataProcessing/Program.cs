using Microsoft.EntityFrameworkCore;
using DataProcessing.Context;
using DataProcessing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OutputService>();
builder.Services.AddScoped<InputService>();

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
