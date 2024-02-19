using System;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace kafka_configuration_lib.Helpers
{
	public static class KafkaEventPublisherHelper
	{
        public static void PublishEvent<TEvent>(IProducer<Null, string> producer, string topic, TEvent eventData)
        {
            var serializedData = SerializeEvent(eventData);
            producer.Produce(topic, new Message<Null, string> { Value = serializedData });
        }

        private static string SerializeEvent<TEvent>(TEvent eventData)
        {
            return JsonConvert.SerializeObject(eventData);
            // You can use any other serialization library or technique here
        }
    }
}

