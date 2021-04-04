using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SimpleRabbit.Common.Configs;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbit.Publisher
{
    public sealed class PublishService : IPublishService
    {
        private const string _defaultSection = "rabbitMq";
        private readonly IRabbitMqConfig _rabbitSettings;

        public PublishService(IConfiguration configuration, string sectionName = _defaultSection) 
        {
            configuration.GetSection(sectionName).Bind(_rabbitSettings);
        }

        private PublishService(IRabbitMqConfig config)
        {
            _rabbitSettings = config;
        }

        private ConnectionFactory _factory;
        private ConnectionFactory Factory => _factory ??=  new ConnectionFactory
        {
            UserName = _rabbitSettings.UserName,
            Password = _rabbitSettings.Password,
            VirtualHost = _rabbitSettings.VirtualHost,
            HostName = _rabbitSettings.HostName,
            Port = _rabbitSettings.Port,
            //ContinuationTimeout = _rabbitSettings.ContinuationTimeout > 0 ? TimeSpan.FromSeconds(_rabbitSettings.ContinuationTimeout) : TimeSpan.FromSeconds(10),
            //AutomaticRecoveryEnabled = _rabbitSettings.AutomaticRecoveryEnabled,
            //NetworkRecoveryInterval = _rabbitSettings.NetworkRecoveryInterval > 0 ? TimeSpan.FromSeconds(_rabbitSettings.NetworkRecoveryInterval) : TimeSpan.FromSeconds(10),
            RequestedHeartbeat = new TimeSpan(0,0,60),
        };

        public static PublishService Create(IRabbitMqConfig settings)
        {
            var srv = new PublishService(settings);
            return srv;
        }

        public async Task<bool> PublishAsync<T>(T message, string routingKey = "")
        {
            if (message is null)
            {
                throw new ArgumentException($"{nameof(PublishAsync)}: Empty message.");
            }

            return await Task.Run(() => Publish(message, routingKey));
        }

        public bool Publish<T>(T message, string routingKey = "")
        {
            try
            {
                using var connection = Factory.CreateConnection();
                using var channel = connection.CreateModel();

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                var exchange = GetExchange<T>();

                IBasicProperties props = channel.CreateBasicProperties();
                props.ContentType = "text/plain";
                props.DeliveryMode = 2;
                channel.BasicPublish(exchange, routingKey, props, body);
                return true;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private string GetExchange<T>()
        {
            return $"{_rabbitSettings.ExchangePrefix}:{typeof(T)}";
        }
    }
}
