﻿using RabbitMQ.Client;

namespace RabbitMQ
{
    public class RabbitMqConnectionConfigure
    {
        public string HostName { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; }


        public ConnectionFactory ConnectionFactory
        {
            get
            {
                var connectionFactory = new ConnectionFactory()
                {
                    HostName = HostName,
                    Port = Port,
                    UserName = UserName,
                    Password = Password,
                    DispatchConsumersAsync = true
                };
                if (!string.IsNullOrWhiteSpace(this.VirtualHost))
                    connectionFactory.VirtualHost = this.VirtualHost;
                return connectionFactory;
            }
        }
    }
}
