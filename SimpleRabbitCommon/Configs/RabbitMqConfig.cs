
namespace SimpleRabbit.Common.Configs
{
    public class RabbitMqConfig : IRabbitMqConfig
    {
        /// <inheritdoc cref="IRabbitMqConfig.UserName"/>
        public string UserName { get; set; }

        /// <inheritdoc cref="IRabbitMqConfig.Password"/>
        public string Password { get; set; }

        /// <inheritdoc cref="IRabbitMqConfig.VirtualHost"/>
        public string VirtualHost { get; set; }

        /// <inheritdoc cref="IRabbitMqConfig.HostName"/>
        public string HostName { get; set; }

        /// <inheritdoc cref="IRabbitMqConfig.QueueName"/>
        public string QueueName { get; set; }

        /// <inheritdoc cref="IRabbitMqConfig.ExchangePrefix"/>
        public string ExchangePrefix { get; set; } = "SimpleRabbit";
        public string ExchangeType { get; set; } = "";

        /// <inheritdoc cref="IRabbitMqConfig.Port"/>
        public int Port { get; set; }

        public int ContinuationTimeoutSec { get; set; } = 10;

        public int NetworkRecoveryIntervalSec { get; set; } = 10;
    }
}
