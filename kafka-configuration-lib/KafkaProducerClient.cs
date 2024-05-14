using System.Text;
using Confluent.Kafka;
using KP.Lib.Kafka.Configurations;
using KP.Lib.Kafka.Helpers;
using KP.Lib.Kafka.Interfaces;

namespace KP.Lib.Kafka;

public class KafkaProducerClient : IProducerClient
{
    private readonly KafkaOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly KafkaProducerConfig _producerConfig;
    private readonly Type _eventType;
    private IProducer<string, byte[]> _producer;

    public KafkaProducerClient(KafkaOptions options, IServiceProvider serviceProvider)
    {
        _options = options;
        _producerConfig = new KafkaProducerConfig(options);
        _serviceProvider = serviceProvider;
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