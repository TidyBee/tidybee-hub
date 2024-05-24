var builder = WebApplication.CreateBuilder(args);


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
            .WithOrigins(frontendUrl ?? "http://localhost:8080")
            .AllowCredentials());
});

builder.Services.AddScoped<OutputService>();
builder.Services.AddScoped<InputService>();

var app = builder.Build();

app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.MapHub<WidgetHub>("/widgetHub");

app.Run();
