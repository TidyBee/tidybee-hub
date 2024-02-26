using AspNetCore.Proxy;
using Microsoft.AspNetCore.Http.Extensions;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
    policy  => {
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

var configuration = builder.Configuration;

try
{
    var aothServiceUrl = configuration.GetValue<string>("AothServiceUrl");
    var dataProcessingServiceUrl = configuration.GetValue<string>("DataProcessingServiceUrl");
    if (aothServiceUrl == null || dataProcessingServiceUrl == null)
        throw new Exception();
    builder.Services.AddHttpClient("AuthServiceClient", client =>
    {
        client.BaseAddress = new Uri(aothServiceUrl);
    });

    builder.Services.AddHttpClient("DataProcessingServiceClient", client =>
    {
        client.BaseAddress = new Uri(dataProcessingServiceUrl);
    });
}
catch (Exception)
{
    throw new ArgumentNullException("Services Urls are not set in configuration.");
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