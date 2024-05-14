using System;
using System.Text;
using Newtonsoft.Json;

namespace KP.Lib.Kafka.Helpers
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

