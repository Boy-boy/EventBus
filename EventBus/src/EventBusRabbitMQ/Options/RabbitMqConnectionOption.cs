using RabbitMQ.Client;

namespace EventBusRabbitMQ.Options
{
    public class RabbitMqConnectionOption
    {
        public ConnectionFactory ConnectionFactory { get; set; }
    }
}
