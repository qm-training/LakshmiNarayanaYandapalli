using WelfareWorkTrackerAuth.Api.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuraion = builder.Configuration;

builder.Services.RegisterSystemServices(configuraion);
builder.Services.RegisterApplicationServices();
builder.Services.RegisterDatabases(configuraion);
builder.Services.RegisterConfigurationServices(configuraion);

var app = builder.Build();

app.CreateMiddlewarePipeline();

await app.RunAsync();