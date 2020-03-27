# EventBus
使用RabbitMq实现EventBus


# 使用方式：
## Publish

```
services.AddEventBusRabbitMq(option =>
            {
            ///添加RabbitMq的连接
                option.RabbitMqConnectionOption = new RabbitMqConnectionOption()
                {
                    ConnectionFactory = new ConnectionFactory()
                    {
                        HostName = "127.0.0.1",
                        VirtualHost = "/",
                        DispatchConsumersAsync = true,
                        UserName = "guest",
                        Password = "guest"
                    }
                };
                ///指定rabbitMq的ExchangeName
                option.RabbitMqPublishOptions=new List<RabbitMqPublishOption>
                {
                    new RabbitMqPublishOption(typeof(UserLocationUpdatedIntegrationEvent), "UserLocationUpdatedIntegrationEvent")
                };
            });
  ``` 
## subscribe
```
 services.AddEventBusRabbitMq(option =>
            {
              ///添加RabbitMq的连接
                option.RabbitMqConnectionOption = new RabbitMqConnectionOption()
                {
                    ConnectionFactory = new ConnectionFactory()
                    {
                        HostName = "127.0.0.1",
                        VirtualHost = "/",
                        DispatchConsumersAsync = true,
                        UserName = "guest",
                        Password = "guest"
                    }
                };
                  ///指定rabbitMq的ExchangeName和QueueName
                option.RabbitMqSubscribeOptions = new List<RabbitMqSubscribeOption>
                {
                    new RabbitMqSubscribeOption(typeof(UserLocationUpdatedIntegrationEvent),
                        "UserLocationUpdatedIntegrationEvent", "UserLocationUpdatedIntegrationEvent")
                };
                  ///添加IntegrationEvent标签
                option.SubscriptionsIntegrationEventOption = new SubscriptionsIntegrationEventOption()
                    .AddSubscriptionsIntegrationEventOption(typeof(UserLocationUpdatedIntegrationEvent), "publish");
               ;
            });
``` 
