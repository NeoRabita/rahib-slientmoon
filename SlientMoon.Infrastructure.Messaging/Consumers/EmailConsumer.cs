using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SlientMoon.Application.DTOs.Email;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Infrastructure.Persistence.Settings;
using System.Text;
using System.Text.Json;

namespace SlientMoon.Infrastructure.Messaging.Consumers
{
    public class EmailConsumer : BackgroundService
    {
        private readonly RabbitMqSettings _rabbitMq;
        private readonly ILogger<EmailConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection? _connection;
        private IChannel? _channel;

        public EmailConsumer(
            IOptions<APIAppSettings> apiSettings,
            ILogger<EmailConsumer> logger,
            IServiceScopeFactory scopeFactory)
        {
            _rabbitMq = apiSettings.Value.RabbitMq;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMq.Host,
                Port = _rabbitMq.Port,
                UserName = _rabbitMq.UserName,
                Password = _rabbitMq.Password,
                VirtualHost = _rabbitMq.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.ExchangeDeclareAsync(
                exchange: _rabbitMq.DeadLetterExchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(
                queue: _rabbitMq.DeadLetterQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);

            await _channel.QueueBindAsync(
                queue: _rabbitMq.DeadLetterQueue,
                exchange: _rabbitMq.DeadLetterExchange,
                routingKey: _rabbitMq.ConsumerQueue,
                cancellationToken: stoppingToken);

            await _channel.ExchangeDeclareAsync(
                exchange: _rabbitMq.Exchange,
                type: _rabbitMq.ExchangeType,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken);

            var queueArguments = new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", _rabbitMq.DeadLetterExchange },
                { "x-dead-letter-routing-key", _rabbitMq.ConsumerQueue }
            };

            await _channel.QueueDeclareAsync(
                queue: _rabbitMq.ConsumerQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArguments,
                cancellationToken: stoppingToken);

            await _channel.QueueBindAsync(
                queue: _rabbitMq.ConsumerQueue,
                exchange: _rabbitMq.Exchange,
                routingKey: _rabbitMq.ConsumerQueue,
                cancellationToken: stoppingToken);

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: (ushort)_rabbitMq.PrefetchCount, global: false, cancellationToken: stoppingToken);
            
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);

                try
                {
                    _logger.LogInformation("RabbitMQ-dan yeni mesaj gəldi: {Message}", messageJson);

                    var emailRequest = JsonSerializer.Deserialize<EmailRequest>(messageJson);

                    if (emailRequest != null)
                    {
                        using var scope = _scopeFactory.CreateScope();

                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                        await emailService.SendOtpEmailAsync(emailRequest.To, emailRequest.Body);

                        _logger.LogInformation("Mail uğurla istifadəçiyə göndərildi: {To}", emailRequest.To);
                    }

                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Mail göndərilərkən xəta baş verdi! Mesaj DLQ-ya yönləndirilir.");

                    await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: _rabbitMq.ConsumerQueue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation("EmailConsumer [{Queue}] növbəsini dinləyir...", _rabbitMq.ConsumerQueue);

            await Task.Delay(Timeout.Infinite, stoppingToken);

        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null) await _channel.CloseAsync(cancellationToken);
            if (_connection != null) await _connection.CloseAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }
}
