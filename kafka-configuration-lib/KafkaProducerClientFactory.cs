using kafka_configuration_lib.Configurations;

namespace kafka_configuration_lib;

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