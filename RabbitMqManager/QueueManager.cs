using RabbitMqManager.Extensions;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace RabbitMqManager
{
    public sealed class QueueManager : BaseManager, IQueueManager
    {
        #region events 

        public event EventHandler<ShutdownEventArgs> ConnectionShutdownDetailedEvent;
        public event EventHandler ConnectionShutdownEvent;

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            ConnectionShutdownDetailedEvent?.Invoke(sender, e);
            ConnectionShutdownEvent?.Invoke(sender, e);
        }

        public event EventHandler ConnectionRecoveredEvent;

        private void OnConnectionRecovered()
        {
            ConnectionRecoveredEvent?.Invoke(this, new EventArgs());
        }

        #endregion events 


        #region fields
       

        private IPushService _pushService;

        private bool _disabled;
        #endregion fields


        #region Properties
        /// <summary>
        /// Timeout of trying to reconnect (in seconds). 
        /// Recommended that the recipient has this value equal to or less than the sender.
        /// Default to 10 s
        /// </summary>
        public int ReconnectTimeout { get; set; } = 10;

        /// <summary>
        /// Heartbeat timeout to use when negotiating with the server (in seconds).  Default to 60 s
        /// </summary>
        public ushort Heartbeat { get; set; } = 60;

        /// <summary>
        /// Set to false to disable automatic connection recovery. Defaults to true.
        /// </summary>
        public bool AutomaticRecoveryEnabled { get; set; } = true;

        /// <summary>
        /// Amount of time client will wait for before re-trying to recover connection. Default to 10 s
        /// </summary>
        public TimeSpan NetworkRecoveryInterval { get; set; } = TimeSpan.FromSeconds(10);

        //private IModel Channel { get { return _channel == null || !_channel.IsOpen ? _channel = Connection.CreateModel() : _channel; } }
        private IModel Channel
        {
            get
            {
                if (_channel == null || _channel?.IsOpen != true)
                {
                    _channel = Connection?.CreateModel();
                }
                return _channel;
            }
        }

        //private IConnection Connection { get { return _connection == null || !_connection.IsOpen ? _connection = GetConnection() : _connection; } }
        private IConnection Connection
        {
            get
            {
                if (_connection == null || _connection?.IsOpen != true)
                {
                    _connection = GetConnection();
                }
                return _connection;
            }
        }

        private readonly HashSet<string> BindedQueues = new HashSet<string>();
        private readonly Dictionary<string, IConsumer> ConsumersDic = new Dictionary<string, IConsumer>();

        #endregion Properties

        #region .ctor
        private QueueManager() { }

        #endregion .ctor


        #region Methods

        private IConnection GetConnection()
        {
            _factory = new ConnectionFactory
            {
                UserName = UserName,
                Password = Password,
                VirtualHost = VirtualHost,
                HostName = HostName,
                AutomaticRecoveryEnabled = AutomaticRecoveryEnabled,
                NetworkRecoveryInterval = NetworkRecoveryInterval,
                RequestedHeartbeat = Heartbeat
            };
            try
            {
                var newConnection = _factory.CreateConnection();
                newConnection.ConnectionShutdown += ConnectionShutdown;
                return newConnection;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to create connection");
                Thread.Sleep(ReconnectTimeout * 1000);
                return null;
            }
        }

        public void AddConsumer<T>(string queue, Action<T> handling, bool cleanQueue) where T : class
        {
            AddConsumer(queue, string.Empty, handling, cleanQueue);
        }

        public void AddConsumer<T>(string queue, string routingKey, Action<T> handling, bool cleanQueue) where T : class
        {
            if (Channel?.IsOpen != true) throw new IOException("Channel is closed");

            if (string.IsNullOrEmpty(queue)) throw new ArgumentNullException("Queue is empty");
            if (handling == null) throw new ArgumentNullException("No consume metod");

            var exchange = GetExchange<T>();

            if (!BindedQueues.Any(q => q == queue))
            {
                if (cleanQueue)
                {
                    try
                    {
                        Channel.QueuePurge(queue);
                    }
                    catch (Exception e)
                    {
                        _logger.Warn(e, $"Can't purge {queue}");
                    }
                }

                Channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

                SetConsumer(queue);

                BindedQueues.Add(queue);
            }

            Channel.ExchangeDeclare(exchange: exchange, type: "fanout");
            Channel.QueueBind(queue, exchange, routingKey);

            ConsumersDic.Add(exchange, new Consumer<T>(queue, routingKey, handling));
        }

        private void RestartConsumers()
        {
            foreach (var queue in BindedQueues)
            {
                Channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

                foreach (var pair in ConsumersDic.Where(q => q.Value.Queue.Equals(queue)))
                {
                    Channel.ExchangeDeclare(exchange:  pair.Key, type: "fanout");
                    Channel.QueueBind(queue: queue, exchange: pair.Key, routingKey: pair.Value.RoutingKey);
                }

                SetConsumer(queue);
            }
        }

        private void SetConsumer(string queue)
        {
            var basicConsumer = new EventingBasicConsumer(Channel);

            basicConsumer.Received += (model, e) =>
            {
                var message = e.Deserialize();

                ConsumersDic.TryGetValue(ConsumersDic.FirstOrDefault(q => q.Key == e.Exchange).Key, out var c);
                c?.Receive(message);
            };

            Channel.BasicConsume(queue: queue, autoAck: true, consumer: basicConsumer);
        }

        public void RemoveConsumer<T>(string queue, string routingKey = "")
        {
            var exchange = GetExchange<T>();

            Channel.QueueUnbind(queue, exchange, routingKey);
            Channel.ExchangeDelete(exchange: exchange);

            var consumerKey = ConsumersDic.FirstOrDefault(q => q.Key.Equals(exchange)).Key;

            ConsumersDic.Remove(consumerKey);
        }

        public static IQueueManager CreateManager(Action<IQueueManager> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException("No configure method");
            }

            var mngr = new QueueManager();
            configure.Invoke(mngr);

            mngr._pushService = PushService.CreateManager((pusher) =>
            {
                pusher.SetHostName(mngr.HostName);
                pusher.SetPassword(mngr.Password);
                pusher.SetUserName(mngr.UserName);
                pusher.SetVirtualHost(mngr.VirtualHost);
                pusher.SetExchangePrefix(mngr.ExchangePrefix);
            });

            return mngr;
        }
        
        public override bool PushMessage<T>(T message, string routingKey = "") 
        {
            return _pushService.PushMessage(message, routingKey);
        }

        private void ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (Connection != null)
            {
                Connection.ConnectionShutdown -= ConnectionShutdown;
            }

            OnConnectionShutdown(sender, e);

            _logger.Error("Rabbit connection lost.");

            Reconnect();
        }

        private void Reconnect()
        {
            Cleanup();

            while (true && !_disabled)
            {
                try
                {
                    RestartConsumers();
                    _logger.Warn("Reconnected to Rabbit");

                    OnConnectionRecovered();

                    break;
                }
                catch (Exception ex)
                {
                    _logger.Warn("Reconnect to Rabbit failed");
                    Thread.Sleep(ReconnectTimeout * 1000);
                }
            }
        }

        /// <summary>
        /// Purge the queue from messages
        /// </summary>
        public void QueuePurge(string queue)
        {
            Channel.QueuePurge(queue);
        }


        public void Connect()
        {
            _disabled = false;
            Reconnect();
        }

        public void Disable()
        {
            _disabled = true;
            Cleanup();
        }

        private void Cleanup()
        {
            try
            {
                if (Channel != null && Channel.IsOpen)
                {
                    Channel?.Close();
                }

                if (Connection != null && Connection.IsOpen)
                {
                    Connection?.Close();
                }
            }
            catch (IOException ex)
            {
                _logger.Error(ex, "Can't clean connection");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Can't clean connection");
            }
        }

        public override void SetUserName(string userName)
        {
            base.SetUserName(userName);
        }

        public override void SetPassword(string password)
        {
            base.SetPassword(password);
        }

        public override void SetVirtualHost(string virtualHost)
        {
            base.SetVirtualHost(virtualHost);
        }

        public override void SetHostName(string hostName)
        {
            base.SetHostName(hostName);
        }

        public override void SetExchangePrefix(string exchangePrefix)
        {
            base.SetExchangePrefix(exchangePrefix);
        }



        #endregion Methods
    }
}
