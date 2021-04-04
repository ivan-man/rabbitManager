using RabbitMQ.Client;
using System;

namespace SimpleRabbit.Cunsuming
{
    public interface IConsumeManager : IDisposable
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
        void AddConsumer<T>(string queue, Action<T> handling, bool cleanQueue, string exchangeType = "fanout") where T : class;

        /// <summary>
        /// Adding of new consumer.
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="queue">Queue name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="handling">Message handler.</param>
        /// <param name="cleanQueue">Is need to clean queue before adding of consumer.</param>
        void AddConsumer<T>(string queue, string routingKey, Action<T> handling, bool cleanQueue, string exchangeType = "fanout") where T : class;

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
    }
}
