using RabbitMQ.Client;
using System.Threading.Tasks;

namespace RabbitMqManager
{
    public abstract class BaseManager : IBaseManager
    {
        protected IConnection _connection;
        protected IModel _channel;
        protected ConnectionFactory _factory;

        ///<inheritdoc cref="IBaseManager.UserName"/>
        public string UserName { get; protected set; }

        ///<inheritdoc cref="IBaseManager.Password"/>
        public string Password { get; protected set; }

        ///<inheritdoc cref="IBaseManager.VirtualHost"/>
        public string VirtualHost { get; protected set; }

        ///<inheritdoc cref="IBaseManager.HostName"/>
        public string HostName { get; protected set; }

        ///<inheritdoc cref="IBaseManager.Port"/>
        public int Port { get; protected set; } = 5672;

        ///<inheritdoc cref="IBaseManager.ContinuationTimeout"/>
        public int ContinuationTimeout { get; protected set; } = 10;

        private string _exchangePrefix;
        ///<inheritdoc cref="IBaseManager.ExchangePrefix"/>
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

        protected BaseManager() 
        {
        }

        ///<inheritdoc cref="IBaseManager.SetUserName(string)"/>
        public virtual void SetUserName(string userName)
        {
            UserName = userName;
        }

        ///<inheritdoc cref="IBaseManager.SetPassword(string)"/>
        public virtual void SetPassword(string password)
        {
            Password = password;
        }

        ///<inheritdoc cref="IBaseManager.SetVirtualHost(string)"/>
        public virtual void SetVirtualHost(string virtualHost)
        {
            VirtualHost = virtualHost;
        }

        ///<inheritdoc cref="IBaseManager.SetHostName(string)"/>
        public virtual void SetHostName(string hostName)
        {
            HostName = hostName;
        }

        ///<inheritdoc cref="IBaseManager.SetPort(int)"/>
        public virtual void SetPort(int port)
        {
            Port = port;
        }

        ///<inheritdoc cref="IBaseManager.SetContinuationTimeout(int)"/>
        public virtual void SetContinuationTimeout(int timeOut)
        {
            ContinuationTimeout = timeOut;
        }

        ///<inheritdoc cref="IBaseManager.SetExchangePrefix(string)"/>
        public virtual void SetExchangePrefix(string exchangePrefix)
        {
            ExchangePrefix = exchangePrefix;
        }

        protected string GetExchange<T>()
        {
            return $"{ExchangePrefix}:{typeof(T).ToString()}";
        }

        public abstract Task<bool> PushMessageAsync<T>(T message, string routingKey = "");
    }
}
