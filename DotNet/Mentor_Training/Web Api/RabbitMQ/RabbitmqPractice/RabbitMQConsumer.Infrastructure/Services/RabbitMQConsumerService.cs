namespace RabbitMQConsumer.Infrastructure.Services;

public class RabbitMQConsumerService(RabbitMQServices service
    , IOptions<RabbitMQOptions> options, IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private readonly RabbitMQServices _services = service;
    private readonly RabbitMQOptions _options = options.Value;
    private readonly IServiceScopeFactory _scopeFactory = serviceScopeFactory;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("[Consumer] Background worker started...");

        var channel = _services.GetChannel();

        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.Received += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            try
            {
                var student = JsonSerializer.Deserialize<StudentDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (student is null)
                {
                    Console.WriteLine("[Consumer] Invalid student data received.");
                    channel.BasicNack(ea.DeliveryTag, false, false);
                    return;
                }

                using var scope = _scopeFactory.CreateScope();
                var studentService = scope.ServiceProvider.GetRequiredService<IStudentService>();

                var result = await studentService.AddStudentAsync(student);

                Console.WriteLine($"[Consumer] {result}");

                channel.BasicAck(ea.DeliveryTag, false);
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Consumer Processing Error: {ex.Message}");
                channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        channel.BasicConsume(queue: _options.QueueName, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }
}
