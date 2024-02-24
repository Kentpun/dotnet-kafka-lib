using System;
using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace kafka_configuration_lib.Helpers
{
	public static class KafkaEventPublisherHelper
	{
        public static byte[] SerializeEvent(object message)
        {
            string jsonString = JsonConvert.SerializeObject(message);
            return Encoding.UTF8.GetBytes(jsonString);
        }
    }
}

