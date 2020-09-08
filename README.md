# EventBus
使用RabbitMq实现EventBus

appsettings.json文件配置rabbitmq链接信息,例如：
```
"RabbitMqConnectionConfigure": {
    "hostName": "127.0.0.1",
    "userName": "guest",
    "password": "guest",
    "port": "-1",
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
 //配置发布消息时使用的交换器（可不配置）
                        .ConfigRabbitMqPublishConfigures(builder=>
                        {
                            builder.AddRabbitMqPublishConfigure(typeof(DemoEvent),
                                "DemoEvent");
                        });
                });

  ``` 
## subscribe
### 1.在ConfigureServices配置如下代码块
```
  services.AddEventBus()
                .AddEventHandler<TEvent, TEventHandler>()
                .AddRabbitMq(configure =>
                {
 //配置rabbitmq连接信息
                    var connectionConfigure = new RabbitMqConnectionConfigure();
                    Configuration.Bind(typeof(RabbitMqConnectionConfigure).Name, connectionConfigure);
                    configure.ConfigRabbitMqConnectionConfigures(connectionConfigure)
//配置订阅消息时使用的交换器，队列和消息标签（可不配置，若配置:交换器名称和消息标签需要与发布的消息保持一致）
                       .ConfigRabbitMqSubscribeConfigures(builder =>
                        {
                            builder.AddRabbitMqSubscribeConfigure(typeof(DemoEvent),
                                "DemoEvent");
                        });
                });
``` 
### 2.在Configure配置如下代码块
```
 var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
 eventBus.Subscribe<DemoEvent,DemoEventHandler>();
```
