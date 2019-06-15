using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMqManager
{
    public interface IBaseManager
    {
        string UserName { get; }
        string Password { get; }
        string VirtualHost { get; }
        string HostName { get; }

        /// <summary>
        /// Префикс Exchange для бидинга DTO
        /// </summary>
        string ExchangePrefix { get; }

        void SetUserName(string userName);

        void SetPassword(string password);

        void SetVirtualHost(string virtualHost);

        void SetHostName(string hostName);

        void SetExchangePrefix(string exchangePrefix);

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="routingKey"></param>
        /// <returns></returns>
        bool PushMessage<T>(T message, string routingKey = "") where T : class;
    }
}
