
using System;

namespace SimpleRabbit.Common.Configs
{
    public interface IRabbitMqConfig
    {
        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.UserName"/>
        string UserName { get; set; }

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.Password"/>
        string Password { get; set; }

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.VirtualHost"/>
        string VirtualHost { get; set; }

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.HostName"/>
        string HostName { get; set; }

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.Port"/>
        public int Port { get; set; }

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.ContinuationTimeout"/>
        public TimeSpan ContinuationTimeout => new TimeSpan(0, 0, ContinuationTimeoutSec);

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.ContinuationTimeout"/>
        public int ContinuationTimeoutSec { get; set; }

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.NetworkRecoveryInterval"/>
        public TimeSpan NetworkRecoveryInterval => new TimeSpan(0, 0, NetworkRecoveryIntervalSec);

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.NetworkRecoveryInterval"/>
        public int NetworkRecoveryIntervalSec { get; set; }

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.Heartbeat"/>
        public TimeSpan Heartbeat => new TimeSpan(0, 0, HeartbeatSec);

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.Heartbeat"/>
        public int HeartbeatSec { get; set; }

        /// <inheritdoc cref="RabbitMQ.Client.ConnectionFactory.AutomaticRecoveryEnabled"/>
        public bool AutomaticRecoveryEnabled { get; set; }


        /// <summary>
        /// Exchange prefix for DTO binding.
        /// </summary>
        public string ExchangePrefix { get; }

        public string ExchangeType { get; }
    }
}
