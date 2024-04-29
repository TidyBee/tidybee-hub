using TidyEvents.Interceptors;
using TidyEvents.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc().AddServiceOptions<TidyBeeEventsService>(options =>
{
    options.Interceptors.Add<AuthInterceptor>();
});

builder.Services.Configure<AuthInterceptorOption>(builder.Configuration.GetSection("AuthInterceptor"));
builder.Services.AddHttpClient<AuthInterceptor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<TidyBeeEventsService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
