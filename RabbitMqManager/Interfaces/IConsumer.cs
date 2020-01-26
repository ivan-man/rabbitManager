using RabbitMQ.Client.Events;
using System;

namespace RabbitMqManager
{
    /// <summary>
    /// Consumer of messages.
    /// </summary>
    public interface IConsumer
    {
        /// <summary>
        /// Name of queue.
        /// </summary>
        string Queue { get; }

        /// <summary>
        /// Routing key.
        /// </summary>
        string RoutingKey { get; }

        /// <summary>
        /// Receive message.
        /// </summary>
        /// <param name="e">Message.</param>
        void Receive(object e);
    }
}
