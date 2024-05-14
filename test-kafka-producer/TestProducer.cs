using System;
using kafka_configuration_lib;
using kafka_configuration_lib.Configurations;
using kafka_configuration_lib.Examples;
using kafka_configuration_lib.Interfaces;

namespace test_kafka_producer
{
	public class TestProducer
	{
        private readonly IServiceProvider _serviceProvider;
        private readonly KafkaOptions _kafkaOptions;
        private IProducerClient _producerClient;

        private Type _eventType;

        public TestProducer(IServiceProvider serviceProvider, KafkaOptions kafkaOptions)
        {
            _serviceProvider = serviceProvider;
            _kafkaOptions = kafkaOptions;
        }

        public void InitializeClient(Type eventType)
        {
            _eventType = eventType;
            _producerClient = _serviceProvider.GetService<KafkaProducerClientFactory>().CreateClient(_kafkaOptions, _eventType);
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
}

