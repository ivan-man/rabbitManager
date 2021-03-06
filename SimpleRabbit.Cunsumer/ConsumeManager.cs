﻿using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using SimpleRabbit.Common.Configs;
using SimpleRabbit.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleRabbit.Cunsuming
{
    public sealed class ConsumeManager : IConsumeManager
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
        private const string _defaultSection = "rabbitMq";

        private IConnection _connection;
        private IModel _channel;
        private ConnectionFactory _factory;

        private IRabbitMqConfig _rabbitSettings;

        private bool _disabled;

        private readonly HashSet<string> _bindedQueues = new HashSet<string>();

        private readonly Dictionary<string, IConsumer> _consumersDic = new Dictionary<string, IConsumer>();
        #endregion fields

        #region Properties
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

        #endregion Properties


        #region .ctor
        private ConsumeManager(IRabbitMqConfig config)
        {
            _rabbitSettings = config;
        }

        public ConsumeManager(IConfiguration configuration, string sectionName = _defaultSection)
        {
            configuration.GetSection(sectionName).Bind(_rabbitSettings);
        }

        #endregion .ctor


        #region Methods

        /// <summary>
        /// Create new instance of <see cref="IConsumeManager"/>.
        /// </summary>
        /// <param name="configureAction">Initiation method.</param>
        /// <param name="config">Rabbit connection attributes.</param>
        public static IConsumeManager Create(IRabbitMqConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var mngr = new ConsumeManager(config);

            return mngr;
        }

        private IConnection GetConnection()
        {
            _factory = new ConnectionFactory
            {
                UserName = _rabbitSettings.UserName,
                Password = _rabbitSettings.Password,
                VirtualHost = _rabbitSettings.VirtualHost,
                HostName = _rabbitSettings.HostName,
                AutomaticRecoveryEnabled = _rabbitSettings.AutomaticRecoveryEnabled,
                NetworkRecoveryInterval = _rabbitSettings.NetworkRecoveryInterval,
                RequestedHeartbeat = _rabbitSettings.Heartbeat,
            };

            try
            {
                var newConnection = _factory.CreateConnection();
                newConnection.ConnectionShutdown += ConnectionShutdown;
                return newConnection;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <inheritdoc cref="IConsumeManager.AddConsumer{T}(string, Action{T}, bool)"/>
        public void AddConsumer<T>(string queue, Action<T> handling, bool cleanQueue, string exchangeType = "fanout") where T : class
        {
            AddConsumer(queue, string.Empty, handling, cleanQueue, exchangeType);
        }

        /// <inheritdoc cref="IConsumeManager.AddConsumer{T}(string, string, Action{T}, bool)"/>
        public void AddConsumer<T>(string queue, string routingKey, Action<T> handling, bool cleanQueue, string exchangeType = "fanout") where T : class
        {
            if (Channel?.IsOpen != true) throw new IOException("Channel is closed");

            if (string.IsNullOrEmpty(queue)) throw new ArgumentNullException("Queue is empty");
            if (handling == null) throw new ArgumentNullException("No consume metod");

            var exchange = GetExchange<T>();

            if (!_bindedQueues.Any(q => q == queue))
            {
                if (cleanQueue)
                {
                    try
                    {
                        Channel.QueuePurge(queue);
                    }
                    catch (OperationInterruptedException e)
                    {
                        // if no queue do nothing
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }

                Channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

                SetConsumer(queue);

                _bindedQueues.Add(queue);
            }

            Channel.ExchangeDeclare(exchange: exchange, type: exchangeType);
            Channel.QueueBind(queue, exchange, routingKey);

            _consumersDic.Add(exchange, new Consumer<T>(queue, routingKey, handling, exchangeType));
        }

        private void RestartConsumers()
        {
            foreach (var queue in _bindedQueues)
            {
                Channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

                foreach (var pair in _consumersDic.Where(q => q.Value.Queue.Equals(queue)))
                {
                    Channel.ExchangeDeclare(exchange: pair.Key, type: pair.Value.ExchangeType);
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

                _consumersDic.TryGetValue(_consumersDic.FirstOrDefault(q => q.Key == e.Exchange).Key, out var c);
                c?.Receive(message);
            };

            Channel.BasicConsume(queue: queue, autoAck: true, consumer: basicConsumer);
        }

        /// <inheritdoc cref="IConsumeManager.RemoveConsumer{T}(string, string)"/>
        public void RemoveConsumer<T>(string queue, string routingKey = "")
        {
            var exchange = GetExchange<T>();

            Channel.QueueUnbind(queue, exchange, routingKey);
            Channel.ExchangeDelete(exchange: exchange);

            var consumerKey = _consumersDic.FirstOrDefault(q => q.Key.Equals(exchange)).Key;

            _consumersDic.Remove(consumerKey);
        }

        private string GetExchange<T>()
        {
            return $"{_rabbitSettings.ExchangePrefix}:{typeof(T)}";
        }

        private void ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (Connection != null)
            {
                Connection.ConnectionShutdown -= ConnectionShutdown;
            }

            OnConnectionShutdown(sender, e);

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
                    OnConnectionRecovered();

                    break;
                }
                catch
                {
                    Task.Delay(_rabbitSettings.NetworkRecoveryInterval * 1000).Wait();
                }
            }
        }

        /// <summary>
        /// Purge the queue from messages.
        /// </summary>
        public void QueuePurge(string queue)
        {
            Channel.QueuePurge(queue);
        }

        /// <summary>
        /// Connect.
        /// </summary>
        public void Connect()
        {
            _disabled = false;
            Reconnect();
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
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
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion Methods
    }
}
