
using System;

namespace SimpleRabbit.Common.Configs
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

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.ContinuationTimeout"/>
        public TimeSpan ContinuationTimeout => new TimeSpan(0, 0, ContinuationTimeoutSec);

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.ContinuationTimeout"/>
        public int ContinuationTimeoutSec { get; }

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.NetworkRecoveryInterval"/>
        public TimeSpan NetworkRecoveryInterval => new TimeSpan(0, 0, NetworkRecoveryIntervalSec);

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.NetworkRecoveryInterval"/>
        public int NetworkRecoveryIntervalSec { get; }

        /// <summary>
        /// Exchange prefix for DTO binding.
        /// </summary>
        public string ExchangePrefix { get; }

        public string ExchangeType { get; }
    }
}
