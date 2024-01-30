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

builder.Services.AddHttpClient("AuthServiceClient", client =>
{
    client.BaseAddress = new Uri("http://hub-auth");
});

builder.Services.AddHttpClient("DataProcessingServiceClient", client =>
{
    client.BaseAddress = new Uri("http://hub-data-processing");
});

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);
app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();
app.UseMiddleware<ApiGateway.HiveMiddleware.HiveMiddleware>();

// var AgentURL = app.Configuration.GetValue<Uri>("AgentURL");

// app.UseProxies(proxies =>
// {
//     proxies.Map("/proxy/{query}", proxy => proxy.UseHttp(async (context, args) =>
//     {
//         var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

//         logger.LogInformation($"Method: {context.Request.Method}, URL: {context.Request.GetDisplayUrl()}");
//         foreach (var header in context.Request.Headers)
//         {
//             logger.LogInformation($"Header: {header.Key}, Value: {header.Value}");
//         }

//         if (context.Request.ContentLength > 0 && (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put))
//         {
//             context.Request.EnableBuffering();
//             var bodyReader = new StreamReader(context.Request.Body);
//             var bodyContent = await bodyReader.ReadToEndAsync();
//             context.Request.Body.Position = 0;
//             logger.LogInformation($"Body: {bodyContent}");
//         }

//         foreach (var arg in args)
//         {
//             logger.LogInformation($"Key: {arg.Key}, Value: {arg.Value}");
//         }
//         string? query = args["query"].ToString();
//         return $"{AgentURL}{query}";
//     }));
// });

app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();

app.Run();