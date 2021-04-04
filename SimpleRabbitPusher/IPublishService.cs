using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbit.Publisher
{
    public interface IPublishService 
    {
        /// <summary>
        /// Sending of message.
        /// </summary>
        /// <typeparam name="T">Type of DTO.</typeparam>
        /// <param name="message">Serialized message.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns></returns>
        Task<bool> PublishAsync<T>(T message, string routingKey = "");

        /// <summary>
        /// Sending of message.
        /// </summary>
        /// <typeparam name="T">Type of DTO.</typeparam>
        /// <param name="message">Serialized message.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns></returns>
        bool Publish<T>(T message, string routingKey = "");
    }
}
