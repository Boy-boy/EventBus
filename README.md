# EventBus
使用RabbitMq实现EventBus

appsettings.json文件配置rabbitmq链接信息,例如：
```
"RabbitMqConnectionConfigure": {
    "hostName": "127.0.0.1",
    "userName": "guest",
    "password": "guest",
    "port": "-1",
    "dispatchConsumersAsync": true,
    "virtualHost": "/"
  }
``` 

# 使用方式：
## Publish

```
  services.AddEventBus()
                .AddRabbitMq(configure =>
                {
                    var connectionConfigure = new RabbitMqConnectionConfigure();
                    Configuration.Bind(typeof(RabbitMqConnectionConfigure).Name, connectionConfigure);
                    configure.ConfigRabbitMqConnectionConfigures(connectionConfigure)
                        .ConfigRabbitMqPublishConfigures(new List<RabbitMqPublishConfigure>
                        {
                            new RabbitMqPublishConfigure(typeof(UserLocationUpdatedIntegrationEvent),
                                "UserLocationUpdatedIntegrationEventExchange")
                        });
                });
  ``` 
## subscribe
```
  services.AddEventBus()
                .AddRabbitMq(configure =>
                {
                    var connectionConfigure = new RabbitMqConnectionConfigure();
                    Configuration.Bind(typeof(RabbitMqConnectionConfigure).Name, connectionConfigure);
                    configure.ConfigRabbitMqConnectionConfigures(connectionConfigure)
                        .ConfigRabbitMqSubscribeConfigures(new List<RabbitMqSubscribeConfigure>()
                        {
                            new RabbitMqSubscribeConfigure(typeof(UserLocationUpdatedIntegrationEvent), 
                                "UserLocationUpdatedIntegrationEventExchange",eventTag:typeof(UserLocationUpdatedIntegrationEvent).Name)
                        });
                });
``` 
