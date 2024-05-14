using KP.Lib.Kafka.Configurations;

namespace KP.Lib.Kafka;

public class KafkaProducerClientFactory
{
    private readonly IServiceProvider _serviceProvider;

    public KafkaProducerClientFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public KafkaProducerClient CreateClient(KafkaOptions options)
    {
        var client = new KafkaProducerClient(options, _serviceProvider);
        client.InitializeProducer();
        return client;
    }
}