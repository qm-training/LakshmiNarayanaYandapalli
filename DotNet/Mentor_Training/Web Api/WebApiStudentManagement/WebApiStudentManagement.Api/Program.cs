var builder = WebApplication.CreateBuilder(args);

IConfiguration configuraion = builder.Configuration;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuraion)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.RegisterSystemService(configuraion);
builder.Services.RegisterApplicationServices();
builder.Services.RegisterDatabases(configuraion);

var app = builder.Build();

app.CreateMiddlewarePipeline();

app.Run();
