﻿using System;
using SimpleRabbit.Common.Helpers;

namespace SimpleRabbit.Cunsuming
{
    internal class Consumer<T> : IConsumer where T : class
    {
        public Action<T> Handling { get; }
        public string Queue { get; private set; }
        public string RoutingKey { get; private set; }
        public string ExchangeType { get; private set; }

        internal Consumer(string queue, string routingKey, Action<T> handling, string exchangeType = "fanout")
        {
            Handling = handling;
            ExchangeType = exchangeType;
        }

        //ToDo generic?
        public void Receive(object obj)
        {
            var message = DeserializeHelper<T>.Deserialize(obj.ToString());
            if (message != null)
            {
                Handling.Invoke(message);
            }
        }
    }
}
