# EventBus
## 一：使用InMemory实现EventBus
#### 1.在ConfigureServices配置如下代码块
``` 
   services.AddEventBus()
           .AddEventHandler<TEvent, TEventHandler>()
           .AddLocal();
```
#### 2.在Configure配置如下代码块（仅订阅消息时需要添加如下代码块）
```
 var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
 eventBus.Subscribe<TEvent,TEventHandler>();
```



## 二.使用RabbitMq实现EventBus

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

### 使用方式：
#### Publish

```
   services.AddRabbitMq(option =>
            {
                var connection = new RabbitMqConnectionConfigure();
                Configuration.Bind(typeof(RabbitMqConnectionConfigure).Name, connection);
                option.Connection = connection;
            });
    services.AddEventBus()
            .AddRabbitMq(configureOptions =>
             {
               //配置发布消息所使用的交换器（若不配置，将提供默认的交换器）
                configureOptions.AddPublishConfigure(option => { option.ExchangeName = "Customer_Exchange"; });
             });

  ``` 
### subscribe
#### 1.在ConfigureServices配置如下代码块
```
   services.AddRabbitMq(option =>
            {
                var connection = new RabbitMqConnectionConfigure();
                Configuration.Bind(typeof(RabbitMqConnectionConfigure).Name, connection);
                option.Connection = connection;
            });
   services.AddEventBus()
            .AddEventHandler<UserEvent, UserEventHandler>()
            .AddRabbitMq(configureOptions =>
             {
               //配置订阅消息所使用的交换器和队列（若不配置，将提供默认的交换器和队列）
                 configureOptions.AddSubscribeConfigures(options =>
                 {
                     options.Add(new RabbitMqSubscribeConfigure(typeof(UserEvent), "Customer_Exchange", "Customer_Queue"));
                 });
             });
``` 
#### 2.在Configure配置如下代码块（仅订阅消息时需要添加如下代码块）
```
 var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
 eventBus.Subscribe<TEvent,TEventHandler>();
```
