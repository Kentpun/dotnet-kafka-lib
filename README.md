### Usage
Configuration Library to assist on the configuration and registration of Kafka consumer and producer.


### Sample

Annotate consumer method
```
[KafkaConsumer("my-topic")]
public void HandleMessage(object message)
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
	var bootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers");
	var consumerGroupId = builder.Configuration.GetValue<string>("Kafka:ConsumerGroupId");
	KafkaOptions kafkaOptions = new KafkaOptions
	{
	    BootstrapServers = bootstrapServers,
	    ConsumerGroupId = consumerGroupId,
	    Debug = "generic" // or "generic,broker,security"
	};

	builder.Services.AddLogging();
	builder.Services.AddSingleton(kafkaOptions);
	var kafkaConsumerConfig = new KafkaConsumerConfig(kafkaOptions);
	builder.Services.AddSingleton<TestConsumer>();
	builder.Services.UseKafkaConsumer(kafkaOptions, kafkaConsumerConfig);
    }
}
```

Kafka Producer
```

```
