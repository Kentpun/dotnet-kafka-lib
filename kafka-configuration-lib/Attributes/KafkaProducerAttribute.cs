using System;
namespace KP.Lib.Kafka
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class KafkaProducerAttribute : Attribute
	{
		public string Topic { get; set; }
		public KafkaProducerAttribute(string topic)
		{
			this.Topic = topic;
		}
	}
}

