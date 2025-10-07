using LibraryManagementRedis.Api.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuraion = builder.Configuration;
builder.Services.RegisterSystemService(configuraion);
builder.Services.RegisterApplicationServices();
builder.Services.RegisterDatabases(configuraion);

var app = builder.Build();

app.CreateMiddlewarePipeline();

await app.RunAsync();
