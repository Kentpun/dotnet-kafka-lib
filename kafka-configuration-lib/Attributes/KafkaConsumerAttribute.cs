namespace KP.Lib.Kafka
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class KafkaConsumerAttribute : Attribute
    {
        public string Topic { get; set; }
        public Type EventType { get; set; }
        public KafkaConsumerAttribute(string topic, Type eventType)
        {
            this.Topic = topic;
            this.EventType = eventType;
        }
    }
}

