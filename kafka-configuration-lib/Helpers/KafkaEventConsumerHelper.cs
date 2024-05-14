using System;
using System.Text;
using Newtonsoft.Json;

namespace KP.Lib.Kafka.Helpers
{
	public static class KafkaEventConsumerHelper
	{
		public static object DeserializeEvent(Type eventType, byte[] message)
		{
			string jsonString = Encoding.UTF8.GetString(message);
			return JsonConvert.DeserializeObject(jsonString, eventType);
		}
	}
}

