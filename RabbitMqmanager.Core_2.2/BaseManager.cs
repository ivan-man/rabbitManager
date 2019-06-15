using NLog;
using RabbitMQ.Client;

namespace RabbitMqManager
{
    public abstract class BaseManager : IBaseManager
    {
        protected readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        protected IConnection _connection;
        protected IModel _channel;
        protected ConnectionFactory _factory;

        public string UserName { get; protected set; }
        public string Password { get; protected set; }
        public string VirtualHost { get; protected set; }
        public string HostName { get; protected set; }

        private string _exchangePrefix;
        public string ExchangePrefix
        {
            get
            {
                return string.IsNullOrEmpty(_exchangePrefix) ? typeof(QueueManager).Namespace : _exchangePrefix;
            }
            set
            {
                _exchangePrefix = value;
            }
        }

        protected BaseManager() { }

        public virtual void SetUserName(string userName)
        {
            UserName = userName;
        }

        public virtual void SetPassword(string password)
        {
            Password = password;
        }

        public virtual void SetVirtualHost(string virtualHost)
        {
            VirtualHost = virtualHost;
        }

        public virtual void SetHostName(string hostName)
        {
            HostName = hostName;
        }

        public virtual void SetExchangePrefix(string exchangePrefix)
        {
            ExchangePrefix = exchangePrefix;
        }

        protected string GetExchange<T>()
        {
            return $"{ExchangePrefix}:{typeof(T).ToString()}";
        }

        public abstract bool PushMessage<T>(T message, string routingKey = "") where T : class;
    }
}
