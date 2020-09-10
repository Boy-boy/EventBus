namespace RabbitMQ
{
    public class RabbitMqOptions
    {
        public RabbitMqConnectionConfigure Connection { get; }

        public RabbitMqExchangeDeclareConfigure ExchangeDeclare { get; }

        public RabbitMqQueueDeclareConfigure QueueDeclare { get; }

        public RabbitMqOptions()
        {
            Connection = new RabbitMqConnectionConfigure();
            ExchangeDeclare=new RabbitMqExchangeDeclareConfigure();
            QueueDeclare=new RabbitMqQueueDeclareConfigure();
        }
    }
}
