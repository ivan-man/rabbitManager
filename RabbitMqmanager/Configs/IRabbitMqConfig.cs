
namespace RabbitMqManager.Configs
{
    public interface IRabbitMqConfig
    {
        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.UserName"/>
        string UserName { get; }

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.Password"/>
        string Password { get; }

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.VirtualHost"/>
        string VirtualHost { get; }

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.HostName"/>
        string HostName { get; }

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.Port"/>
        public int Port { get; }

        /// <summary>
        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.ContinuationTimeout"/> (ms)
        /// </summary>
        public int ContinuationTimeout { get; }

        /// <summary>
        /// Name of Queue.
        /// </summary>
        string QueueName { get; set; }
        /// <summary>
        /// Exchange prefix for DTO binding.
        /// </summary>
        string ExchangePrefix { get; }
    }
}
