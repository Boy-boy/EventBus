using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace EventBusRabbitMQ.Provider
{
    public class RabbitMqConnectionProvider
    {
        private readonly IOptions<RabbitMqConnectionOption> _options;

        public RabbitMqConnectionProvider(IOptions<RabbitMqConnectionOption> options)
        {
            _options = options;
        }
        public RabbitMqConnectionOption GetRabbitMqConnectionOption()
        {
            return _options.Value;
        }
    }

    public class RabbitMqConnectionOption
    {
        public ConnectionFactory ConnectionFactory { get; set; }
    }
}
