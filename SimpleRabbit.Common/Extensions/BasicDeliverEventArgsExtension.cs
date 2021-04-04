using RabbitMQ.Client.Events;
using SimpleRabbit.Common.Helpers;
using System.Text;

namespace SimpleRabbit.Common.Extensions
{
    public static class BasicDeliverEventArgsExtension
    {
        public static T Deserialize<T>(this BasicDeliverEventArgs e) where T : class
        {
            var bodyString = Encoding.UTF8.GetString(e.Body.ToArray());
            return DeserializeHelper<T>.Deserialize(bodyString);
        }

        public static object Deserialize(this BasicDeliverEventArgs e)
        {
            var bodyString = Encoding.UTF8.GetString(e.Body.ToArray());
            return DeserializeHelper.Deserialize(bodyString);
        }
    }
}
