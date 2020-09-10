using System.Threading.Tasks;

namespace RabbitMQ
{
    public interface IRabbitMqMessageConsumer
    {
        Task BindAsync(string routingKey);

        Task UnbindAsync(string routingKey);
    }
}
