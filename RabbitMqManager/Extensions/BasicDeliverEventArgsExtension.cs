using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMqManager.Helpers;
using System;
using System.Text;

namespace RabbitMqManager.Extensions
{
    public static class BasicDeliverEventArgsExtension
    {
        public static T Deserialize<T>(this BasicDeliverEventArgs e) where T : class
        {
            var bodyString = Encoding.UTF8.GetString(e.Body);
            return DeserializeHelper<T>.Deserialize(bodyString);
        }

        public static object Deserialize(this BasicDeliverEventArgs e)
        {
            var bodyString = Encoding.UTF8.GetString(e.Body);
            return DeserializeHelper.Deserialize(bodyString);
        }
    }
}
