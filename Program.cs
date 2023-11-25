using AspNetCore.Proxy;

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

// Not used right now, Middleware will be used later in the project when AAA (Authorization, Authentification and Accounting) will be started
// app.UseMiddleware<ProxyMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

var AgentURL = app.Configuration.GetValue<Uri>("AgentURL");

app.UseProxies(proxies => {
    proxies.Map("/proxy/{query}", proxy => proxy.UseHttp((_, args) => {
        string? query = args["query"].ToString();
        Console.WriteLine("QUERY " + query);
        return $"{AgentURL}{query}";
    }));
});


app.UseCors(policy => policy
    .AllowAnyOrigin()
       .AllowAnyMethod()
          .AllowAnyHeader());

app.MapControllers();

app.Run();
