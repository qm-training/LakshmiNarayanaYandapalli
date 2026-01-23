var builder = WebApplication.CreateBuilder(args);

IConfiguration configuraion = builder.Configuration;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuraion)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.RegisterSystemServices(configuraion);
builder.Services.RegisterApplicationServices();
builder.Services.RegisterDatabases(configuraion);
builder.Services.RegisterConfigurationServices(configuraion);

var app = builder.Build();

app.CreateMiddlewarePipeline();

await app.RunAsync();