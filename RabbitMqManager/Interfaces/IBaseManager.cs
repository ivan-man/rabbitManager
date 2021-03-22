using System;
using System.Threading.Tasks;

namespace RabbitMqManager
{
    public interface IBaseManager 
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
        /// Exchange prefix for DTO binding.
        /// </summary>
        string ExchangePrefix { get; }

        /// <summary>
        /// Set <see cref="IBaseManager.UserName"/>.
        /// </summary>
        /// <param name="userName">Username.</param>
        void SetUserName(string userName);

        /// <summary>
        /// Set <see cref="IBaseManager.Password"/>.
        /// </summary>
        /// <param name="password">Password.</param>
        void SetPassword(string password);

        /// <summary>
        /// Set <see cref="IBaseManager.VirtualHost"/>.
        /// </summary>
        /// <param name="virtualHost">Viirual host name.</param>
        void SetVirtualHost(string virtualHost);

        /// <summary>
        /// Set <see cref="IBaseManager.HostName"/>.
        /// </summary>
        /// <param name="hostName">Hostname.</param>
        void SetHostName(string hostName);

        /// <summary>
        /// Set <see cref="IBaseManager.ExchangePrefix"/>.
        /// </summary>
        /// <param name="exchangePrefix">Prefix of exchange.</param>
        void SetExchangePrefix(string exchangePrefix);

        /// <summary>
        /// Set <see cref="IBaseManager.Port"/>.
        /// </summary>
        /// <param name="port">Port number.</param>
        void SetPort(int port);

        /// <summary>
        /// Set <see cref="IBaseManager.ContinuationTimeout"/>.
        /// </summary>
        /// <param name="timeOut"></param>
        void SetContinuationTimeout(int timeOut);

        /// <summary>
        /// Sending of message.
        /// </summary>
        /// <typeparam name="T">Type of DTO.</typeparam>
        /// <param name="message">Serialized message.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns></returns>
        Task<bool> PushMessageAsync<T>(T message, string routingKey = "");
    }
}
