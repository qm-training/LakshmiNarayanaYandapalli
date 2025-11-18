namespace Notification.Api.Infrastructure.Extensions;
public static class ServiceCollectionExtentions
{
    public static void RegisterSystemServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            // Add your consumer here
            x.AddConsumer<EmailReceiverService>();
            var queueConfig = configuration.GetSection("QueueConfig").Get<QueueConfig>() ?? throw new InvalidOperationException("QueueConfig is not configured properly in the configuration file.");
            x.UsingRabbitMq((context, cfg) =>
            {
                // Configuring the RabbitMQ host
                cfg.Host(queueConfig.Uri, h =>
                {
                    h.Username(queueConfig.UserName);
                    h.Password(queueConfig.Password);
                });
                // Configuring the receive endpoint
                cfg.ReceiveEndpoint("WelfareQueue", ep =>
                {
                    // Disable auto-acknowledgment
                    ep.PrefetchCount = 1; // To handle one message at a time, prevents unacknowledged floods
                    // Use manual acknowledgment
                    ep.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(10))); // Retry 3 times with a 10-second interval
                    // Circuit Breaker configuration
                    ep.UseCircuitBreaker(cb =>
                    {
                        cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                        cb.TripThreshold = 15;
                        cb.ActiveThreshold = 10;
                        cb.ResetInterval = TimeSpan.FromMinutes(5);
                    });
                    // Configuring the consumer
                    ep.ConfigureConsumer<EmailReceiverService>(context);
                });
            });
        });
    }

    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<EmailReceiverService>();
        services.AddScoped<ISmtpClientWrapper, SmtpClientWrapperService>();
    }
    public static void RegisterDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        //No Database to Register
    }

    public static void RegisterConfigurationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpClientOptions>(configuration.GetSection("SmtpClientOptions"));
    }
}