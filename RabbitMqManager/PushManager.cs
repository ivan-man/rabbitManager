using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMqManager
{
    internal sealed class PushService : BaseManager, IPushService
    {
        private ConnectionFactory Factory => new ConnectionFactory
        {
            UserName = UserName,
            Password = Password,
            VirtualHost = VirtualHost,
            HostName = HostName,
            Port = Port,
            ContinuationTimeout = TimeSpan.FromMilliseconds(ContinuationTimeout),
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
            RequestedHeartbeat = 60,
        };
        //{
        //    get
        //    {
        //        return _factory ?? (_factory = new ConnectionFactory
        //        {
        //            UserName = UserName,
        //            Password = Password,
        //            VirtualHost = VirtualHost,
        //            HostName = HostName,
        //            Port = Port,
        //            ContinuationTimeout = TimeSpan.FromMilliseconds(ContinuationTimeout),
        //            AutomaticRecoveryEnabled = true,
        //            NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
        //            RequestedHeartbeat = 60,
        //        });
        //    }
        //}

        private PushService() : base()
        { }

        public static PushService CreateManager(Action<IPushService> configure)
        {
            var mngr = new PushService();

            configure.Invoke(mngr);

            return mngr;
        }

        public async override Task<bool> PushMessageAsync<T>(T message, string routingKey = "")
        {
            if (message is null)
            {
                throw new ArgumentException($"{nameof(PushMessageAsync)}: Empty message.");
            }

            return await Task.Run(() => PushMessage(message, routingKey));
        }

        private bool PushMessage<T>(T message, string routingKey = "")
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
                throw;
            }
        }
    }
}
