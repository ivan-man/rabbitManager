using Newtonsoft.Json;
using NLog;
using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMqManager
{
    internal sealed class PushService : BaseManager, IPushService
    {
        private ConnectionFactory Factory
        {
            get
            {
                return _factory ?? (_factory = new ConnectionFactory
                {
                    UserName = UserName,
                    Password = Password,
                    VirtualHost = VirtualHost,
                    HostName = HostName,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                    RequestedHeartbeat = 60
                });
            }
        }

        private PushService() { }

        public static PushService CreateManager(Action<IPushService> configure)
        {
            var mngr = new PushService();

            configure.Invoke(mngr);

            return mngr;
        }

        public override bool PushMessage<T>(T message, string routingKey = "") 
        {
            try
            {
                using (var connection = Factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                    var exchange = GetExchange<T>();

                    IBasicProperties props = channel.CreateBasicProperties();
                    props.ContentType = "text/plain";
                    props.DeliveryMode = 2;
                    channel.BasicPublish(exchange, routingKey, props, body);
                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return false;
            }
        }
    }
}
