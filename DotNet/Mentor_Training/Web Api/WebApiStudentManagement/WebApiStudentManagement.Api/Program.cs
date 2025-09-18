var builder = WebApplication.CreateBuilder(args);

IConfiguration configuraion = builder.Configuration;

//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();
//builder.Logging.SetMinimumLevel(LogLevel.Information);

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
