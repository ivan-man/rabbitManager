using RabbitMQ.Client;
using System;

namespace RabbitMqManager
{
    public interface IQueueManager : IBaseManager, IDisposable
    {
        /// <summary>
        /// Detailed lost connection with bus.
        /// </summary>
        event EventHandler<ShutdownEventArgs> ConnectionShutdownDetailedEvent;

        /// <summary>
        /// Lost connection with bus.
        /// </summary>
        event EventHandler ConnectionShutdownEvent;

        /// <summary>
        /// Connection restored.
        /// </summary>
        event EventHandler ConnectionRecoveredEvent;

        /// <summary>
        /// Timeout of trying to reconnect (in seconds). 
        /// Recommended that the recipient has this value equal to or less than the sender.
        /// Default to 10s.
        /// </summary>
        int ReconnectTimeout { get; set; }

        /// <summary>
        /// Heartbeat timeout to use when negotiating with the server (in seconds).  Default to 60s.
        /// </summary>
        TimeSpan Heartbeat { get; set; }

        /// <summary>
        /// Set to false to disable automatic connection recovery. Defaults to true.
        /// </summary>
        bool AutomaticRecoveryEnabled { get; set; }

        /// <summary>
        /// Amount of time client will wait for before re-trying to recover connection. Default to 10s.
        /// </summary>
        TimeSpan NetworkRecoveryInterval { get; set; }

        /// <summary>
        /// Purge the queue from messages.
        /// </summary>
        /// <param name="queue"></param>
        void QueuePurge(string queue);

        /// <summary>
        /// Adding of new consumer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue">Queue name.</param>
        /// <param name="handling">Message handler.</param>
        /// <param name="cleanQueue">Is need to clean queue before adding of consumer.</param>
        void AddConsumer<T>(string queue, Action<T> handling, bool cleanQueue) where T : class;

        /// <summary>
        /// Adding of new consumer.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="queue">Queue name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="handling">Message handler.</param>
        /// <param name="cleanQueue">Is need to clean queue before adding of consumer.</param>
        void AddConsumer<T>(string queue, string routingKey, Action<T> handling, bool cleanQueue) where T : class;

        /// <summary>
        /// Удаление обработчика
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="queue">Queue name./param>
        /// <param name="routingKey">Routing key.</param>
        void RemoveConsumer<T>(string queue, string routingKey = "");

        /// <summary>
        /// Connect.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disable connection.
        /// </summary>
        void Dispose();
    }
}
