### Usage
Configuration Library to assist on the configuration and registration of Kafka consumer and producer.


### Sample

Annotate consumer method
```
[KafkaConsumer("my-topic")]
public void HandleMessage(string message)
{
    // Process the consumed message
    Console.WriteLine($"Received message: {message}");
}
```

Register Kafka Consumer
```
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Configure other services...

        var kafkaConsumerConfig = new KafkaConsumerConfig("localhost:9092", "my-consumer-group");
        services.AddKafkaConsumers(kafkaConsumerConfig);
    }
}
```

Kafka Producer
```

```
