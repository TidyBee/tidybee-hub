var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ProxyMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(policy => policy
    .AllowAnyOrigin()
       .AllowAnyMethod()
          .AllowAnyHeader());

app.MapControllers();

app.Run();
