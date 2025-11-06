using MassTransit;
using Polly;
using Polly.Retry;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using WelfareWorkTracker.Core.Contracts.Service;

namespace WelfareWorkTracker.Infrastructure.Service
{
    public class EmailTransmitterService : IEmailTransmitterService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        readonly int maxRetryAttempts = 3;
        readonly TimeSpan pauseBetweenFailures = TimeSpan.FromSeconds(5);
        readonly AsyncRetryPolicy retryPolicy;

        public EmailTransmitterService(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
            retryPolicy = Policy.Handle<RabbitMqConnectionException>()
                               .Or<MessageNotAcknowledgedException>()
                               .Or<BrokerUnreachableException>()
                               .Or<SocketException>()
                               .WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures);
        }

        public async Task<bool> Publish<T>(T customerData) where T : class
        {
            return await ExecutePublish(async () => await _publishEndpoint.Publish(customerData));
        }

        public async Task<bool> PublishBatch<T>(IList<T> customerData) where T : class
        {
            return await ExecutePublish(async () => await _publishEndpoint.PublishBatch(customerData));
        }

        private async Task<bool> ExecutePublish(Func<Task> publishAction)
        {
            try
            {
                await retryPolicy.ExecuteAsync(publishAction);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
