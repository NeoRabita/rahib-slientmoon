
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Infrastructure.Persistence.Settings;
using System.Text;
using System.Text.Json;

namespace SlientMoon.Infrastructure.Messaging.Services
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly RabbitMqSettings _rabbitMqSettings;

        public RabbitMqPublisher(IOptions<APIAppSettings> apiSettings)
        {
            _rabbitMqSettings = apiSettings.Value.RabbitMq;
        }


        public async Task PublishAsync<T>(T message, string routingKey) where T : class
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMqSettings.Host,
                Port = _rabbitMqSettings.Port,
                UserName = _rabbitMqSettings.UserName,
                Password = _rabbitMqSettings.Password,
                VirtualHost = _rabbitMqSettings.VirtualHost
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: _rabbitMqSettings.Exchange,
                type: _rabbitMqSettings.ExchangeType,
                durable: true,
                autoDelete: false);

            if (!string.IsNullOrEmpty(_rabbitMqSettings.DeadLetterExchange))
            {
                await channel.ExchangeDeclareAsync(
                    exchange: _rabbitMqSettings.DeadLetterExchange,
                    type: ExchangeType.Direct,
                    durable: true,
                    autoDelete: false);
            }

            var queueArguments = new Dictionary<string, object?>();
            if (!string.IsNullOrEmpty(_rabbitMqSettings.DeadLetterExchange))
            {
                queueArguments.Add("x-dead-letter-exchange", _rabbitMqSettings.DeadLetterExchange);
                queueArguments.Add("x-dead-letter-routing-key", _rabbitMqSettings.ConsumerQueue);
            }

            await channel.QueueDeclareAsync(
                queue: _rabbitMqSettings.ConsumerQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArguments);

            await channel.QueueBindAsync(
                queue: _rabbitMqSettings.ConsumerQueue,
                exchange: _rabbitMqSettings.Exchange,
                routingKey: routingKey);


            if (!string.IsNullOrEmpty(_rabbitMqSettings.DeadLetterQueue) && !string.IsNullOrEmpty(_rabbitMqSettings.DeadLetterExchange))
            {
                await channel.QueueDeclareAsync(
                    queue: _rabbitMqSettings.DeadLetterQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                await channel.QueueBindAsync(
                    queue: _rabbitMqSettings.DeadLetterQueue,
                    exchange: _rabbitMqSettings.DeadLetterExchange,
                    routingKey: _rabbitMqSettings.ConsumerQueue);
            }

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            
            
            var properties = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent 
            };

            await channel.BasicPublishAsync(
                exchange: _rabbitMqSettings.Exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body);

        }
    }
}
