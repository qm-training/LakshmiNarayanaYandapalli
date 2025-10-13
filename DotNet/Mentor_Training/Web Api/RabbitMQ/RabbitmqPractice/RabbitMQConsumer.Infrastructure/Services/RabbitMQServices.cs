using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQConsumer.Core.Options;

namespace RabbitMQConsumer.Infrastructure.Services
{
    public class RabbitMQServices : IAsyncDisposable
    {
        private readonly RabbitMQOptions _options;
        private readonly IConnection _connection;
        private readonly RabbitMQ.Client.IModel _channel;

        public RabbitMQServices(IOptions<RabbitMQOptions> options)
        {
            _options = options.Value;
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                DispatchConsumersAsync = true
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_options.ExchangeName, ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare(_options.QueueName, true, false, false, null);
            _channel.QueueBind(_options.QueueName, _options.ExchangeName, _options.RoutingKey);
        }

        public RabbitMQ.Client.IModel GetChannel() => _channel;

        public ValueTask DisposeAsync()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
