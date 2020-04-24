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
                   //配置rabbitmq连接信息
                    var connectionConfigure = new RabbitMqConnectionConfigure();
                    Configuration.Bind(typeof(RabbitMqConnectionConfigure).Name, connectionConfigure);
                    configure.ConfigRabbitMqConnectionConfigures(connectionConfigure)
                    //设置发布消息时使用的交换器（可不配置）
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
                  //配置rabbitmq连接信息
                    var connectionConfigure = new RabbitMqConnectionConfigure();
                    Configuration.Bind(typeof(RabbitMqConnectionConfigure).Name, connectionConfigure);
                    configure.ConfigRabbitMqConnectionConfigures(connectionConfigure)
                     //配置订阅消息时使用的交换器，队列和消息标签（可不配置，若配置交换器名称和消息标签需要与发布消息保持一致）
                        .ConfigRabbitMqSubscribeConfigures(new List<RabbitMqSubscribeConfigure>()
                        {
                            new RabbitMqSubscribeConfigure(typeof(UserLocationUpdatedIntegrationEvent), 
                                "UserLocationUpdatedIntegrationEventExchange",eventTag:typeof(UserLocationUpdatedIntegrationEvent).Name)
                        });
                });
``` 
