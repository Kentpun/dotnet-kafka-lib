### Usage
This Configuration Library to assist on the configuration and registration of Kafka consumer and producer.


### Details

#### Annotate consumer method

The annotation/attribute takes two parameters:
```
1. topic name: string
2. Message type: Type
```
Sample code:
```
[KafkaConsumer("test-topic", typeof(TestClass))]
public void HandleMessage(TestClass message)
{
    Console.WriteLine($"Received message: {message.id}: {message.message}");
    // Your processing logic here
}
```

#### Register Kafka Consumer

The below sample ASP.NET core configuration illustrates the necessary steps and configuration to register and run Kafka Consumers.  
```
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var bootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers");
        var consumerGroupId = builder.Configuration.GetValue<string>("Kafka:ConsumerGroupId");
        bool useSasl = builder.Configuration.GetValue<bool>("Kafka:UseSasl");
        var saslMechanism = builder.Configuration.GetValue<string>("Kafka:SaslMechanism");
        var securityProtocol = builder.Configuration.GetValue<string>("Kafka:SecurityProtocol");
        var saslUsername = builder.Configuration.GetValue<string>("Kafka:SaslUsername");
        var saslPassword = builder.Configuration.GetValue<string>("Kafka:SaslPassword");

        KafkaOptions kafkaOptions = new KafkaOptions
        {
            BootstrapServers = bootstrapServers,
            ConsumerGroupId = consumerGroupId,
            Debug = "generic" // or "generic,broker,security"
            UseSasl = useSasl,
            SaslMechanism = saslMechanism,
            SecurityProtocol = securityProtocol,
            SaslUsername = saslUsername,
            SaslPassword = saslPassword
        };
        
        builder.Services.AddLogging();
        builder.Services.AddSingleton<TestConsumer>();
        builder.Services.UseKafkaConsumer(kafkaOptions);
    }
}
```

#### Kafka Producer
The below sample ASP.NET core code illustrates the steps and configuration to register a producer.

Configuration:
```
var bootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers");
var clientId = builder.Configuration.GetValue<string>("Kafka:ClientId");
bool useSasl = builder.Configuration.GetValue<bool>("Kafka:UseSasl");
var saslMechanism = builder.Configuration.GetValue<string>("Kafka:SaslMechanism");
var securityProtocol = builder.Configuration.GetValue<string>("Kafka:SecurityProtocol");
var saslUsername = builder.Configuration.GetValue<string>("Kafka:SaslUsername");
var saslPassword = builder.Configuration.GetValue<string>("Kafka:SaslPassword");
KafkaOptions kafkaOptions = new KafkaOptions
{
    BootstrapServers = bootstrapServers,
    ClientId = clientId,
    Debug = "generic" // or "generic,broker,security",
    UseSasl = useSasl,
    SaslMechanism = saslMechanism,
    SecurityProtocol = securityProtocol,
    SaslUsername = saslUsername,
    SaslPassword = saslPassword
};

builder.Services.UseKafkaProducer();
builder.Services.AddSingleton(kafkaOptions);
builder.Services.AddSingleton<Producer>();
```

Producer (Sample producer service):
```
public class Producer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly KafkaOptions _kafkaOptions;
    private IProducerClient _producerClient;
    
    private Type _eventType;
    
    public Producer(IServiceProvider serviceProvider, KafkaOptions kafkaOptions)
    {
        _serviceProvider = serviceProvider;
        _kafkaOptions = kafkaOptions;
    }

    public void InitializeClient(Type eventType)
    {
        _eventType = eventType;
        _producerClient = _serviceProvider.GetService<KafkaProducerClientFactory>().CreateClient(_kafkaOptions);
    }
    
    public void TestPublish(TestClass data)
    {
        try
        {
            _producerClient.Produce("test-topic", data, TimeSpan.FromMilliseconds(1000));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to publish message: {ex.Message}");
        }
        
    }
}
```
