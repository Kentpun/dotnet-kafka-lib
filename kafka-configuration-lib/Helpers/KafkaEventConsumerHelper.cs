using System;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace kafka_configuration_lib.Helpers
{
	public static class KafkaEventConsumerHelper
	{
		public static TEvent DeserializeEvent<TEvent>(ConsumeResult<Ignore, string> message)
		{
			var jsonString = message.Message.Value;
			var eventData = JsonConvert.DeserializeObject<TEvent>(jsonString);
			return eventData;
		}
	}
}

