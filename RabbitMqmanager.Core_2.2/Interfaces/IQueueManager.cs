using RabbitMQ.Client;
using System;

namespace RabbitMqManager
{
    public interface IQueueManager : IBaseManager
    {
        /// <summary>
        /// Детализированное cобытие потери соединения с шиной
        /// </summary>
        event EventHandler<ShutdownEventArgs> ConnectionShutdownDetailedEvent;

        /// <summary>
        /// Потеря соединения с шиной
        /// </summary>
        event EventHandler ConnectionShutdownEvent;

        /// <summary>
        /// Соединение восстановленно
        /// </summary>
        event EventHandler ConnectionRecoveredEvent;

        /// <summary>
        /// Timeout of trying to reconnect (in seconds). 
        /// Recommended that the recipient has this value equal to or less than the sender.
        /// Default to 10 s
        /// </summary>
        int ReconnectTimeout { get; set; }

        /// <summary>
        /// Heartbeat timeout to use when negotiating with the server (in seconds).  Default to 60 s
        /// </summary>
        ushort Heartbeat { get; set; }

        /// <summary>
        /// Set to false to disable automatic connection recovery. Defaults to true.
        /// </summary>
        bool AutomaticRecoveryEnabled { get; set; }

        /// <summary>
        /// Amount of time client will wait for before re-trying to recover connection. Default to 10 s
        /// </summary>
        TimeSpan NetworkRecoveryInterval { get; set; }

        /// <summary>
        /// Purge the queue from messages
        /// </summary>
        /// <param name="queue"></param>
        void QueuePurge(string queue);

        /// <summary>
        /// Добавление обработчика 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue">Имя очереди</param>
        /// <param name="handling">Метод обработчика</param>
        /// <param name="cleanQueue">Очистка очереди перед добавлением</param>
        void AddConsumer<T>(string queue, Action<T> handling, bool cleanQueue) where T : class;

        /// <summary>
        /// Добавление обработчика 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue">Имя очереди</param>
        /// <param name="routing"></param>
        /// <param name="handling">Метод обработчика</param>
        /// <param name="cleanQueue">Очистка очереди перед добавлением</param>
        void AddConsumer<T>(string queue, string routing, Action<T> handling, bool cleanQueue) where T : class;

        /// <summary>
        /// Удаление обработчика
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue">Имя очереди</param>
        /// <param name="routingKey"></param>
        void RemoveConsumer<T>(string queue, string routingKey = "");

        void Connect();
        void Disable();
    }
}
