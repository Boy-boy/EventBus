using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQ
{
    public class ChannelPool
    {
        private readonly IRabbitMqPersistentConnection _connection;

        protected ConcurrentDictionary<string, IModel> Channels { get; }
        public ChannelPool(IRabbitMqPersistentConnection connection)
        {
            _connection = connection;
            Channels=new ConcurrentDictionary<string, IModel>();
        }


        public IModel CreateModel()
        {
            if (!_connection.IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }
            return _connection.CreateModel();
        }
    }
}
