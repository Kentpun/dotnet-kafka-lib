using System.Text;
using Confluent.Kafka;
using kafka_configuration_lib.Configurations;
using kafka_configuration_lib.Helpers;
using kafka_configuration_lib.Interfaces;

namespace kafka_configuration_lib;

public class KafkaProducerClient : IProducerClient
{
    private readonly KafkaOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly KafkaProducerConfig _producerConfig;
    private readonly Type _eventType;
    private IProducer<string, byte[]> _producer;

    public KafkaProducerClient(KafkaOptions options, IServiceProvider serviceProvider, KafkaProducerConfig producerConfig, Type eventType)
    {
        _options = options;
        _producerConfig = producerConfig;
        _serviceProvider = serviceProvider;
        _eventType = eventType;
    }

    
    public void InitializeProducer()
    {
        _producer = new ProducerBuilder<string, byte[]>(_producerConfig.ProducerConfig)
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            .SetStatisticsHandler((_, json) => Console.WriteLine($"Statistics: {json}"))
            .Build();
    }

    public void Produce(string topic, object data,  TimeSpan timeout)
    {
        try
        {
            var jsonText = KafkaEventPublisherHelper.SerializeEvent(data);
            var messageType = data.GetType().FullName; // Get the full name of the type
            var headers = new Headers();
            headers.Add("MessageType", Encoding.UTF8.GetBytes(messageType));
            var message = new Message<string, byte[]>
            {
                Key = "message-1", // Your key
                Value = jsonText,
                Headers = headers
            };
            _producer.Produce(topic, message);
            _producer.Flush(timeout);
        }
        catch (Exception ex)
        {
            // Handle exception
            Console.WriteLine($"Error producing message: {ex.Message}");
        }
        
        
        

        
    }
}