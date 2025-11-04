var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.json", false, true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

await Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    IConfiguration configuration = builder.Configuration;
                    
                    services.RegisterApplicationServices();
                    services.RegisterSystemServices(configuration);
                    services.RegisterConfigurationServices(configuration);
                })
                .UseSerilog()
                .Build()
                .RunAsync();