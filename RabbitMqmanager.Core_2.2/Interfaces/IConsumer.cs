using RabbitMQ.Client.Events;
using System;

namespace RabbitMqManager
{
    public interface IConsumer
    {
        string Queue { get; }
        string RoutingKey { get; }

        void Receive(object e);
    }
}
