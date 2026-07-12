using System.Collections.Generic;

namespace SlientMoon.Infrastructure.Persistence.Settings
{
    public class RabbitMqSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Exchange { get; set; }
        public string ExchangeType { get; set; }
        public string DeadLetterExchange { get; set; }
        public string ConsumerQueue { get; set; }
        public string DeadLetterQueue { get; set; }
        public List<string> RoutingKeys { get; set; }
        public int PrefetchCount { get; set; }
        public int ConnectionRetryCount { get; set; }
        public int ConnectionRetryBaseDelaySeconds { get; set; }
    }
}
