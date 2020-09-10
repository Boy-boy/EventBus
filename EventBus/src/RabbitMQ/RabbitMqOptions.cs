namespace RabbitMQ
{
    public class RabbitMqOptions
    {
        public RabbitMqConnectionConfigure Connection { get; }

        public RabbitMqOptions()
        {
            Connection = new RabbitMqConnectionConfigure();
        }
    }
}
