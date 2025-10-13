using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQSender.Core.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace RabbitMQSender.Infrastructure.Services
{
    public class RabbitMQServices : IAsyncDisposable
    {
        private readonly RabbitMQOptions _options;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQServices(IOptions<RabbitMQOptions> options)
        {
            _options = options.Value;
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_options.ExchangeName, ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare(_options.QueueName, true, false, false, null);
            _channel.QueueBind(_options.QueueName, _options.ExchangeName, _options.RoutingKey);
        }

        public async Task PublishAsync(object message)
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(message);

            await Task.Run(() =>
            {
                _channel.BasicPublish(
                    exchange: _options.ExchangeName,
                    routingKey: _options.RoutingKey,
                    basicProperties: null,
                    body: body);
            });

            Console.WriteLine($"[Producer] Published → {message}");
        }

        public IModel GetChannel() => _channel;

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
