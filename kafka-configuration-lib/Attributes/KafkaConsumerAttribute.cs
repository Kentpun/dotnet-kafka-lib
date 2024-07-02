namespace KP.Lib.Kafka
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class KafkaConsumerAttribute : Attribute
    {
        public string Topic { get; set; }
        public Type? EventType { get; set; }
        public string? SchemaRegistryUrl { get; set; }
        public KafkaConsumerAttribute(string topic, Type? eventType, string? schemaRegistryUrl)
        {
            this.Topic = topic;
            this.EventType = eventType;
            this.SchemaRegistryUrl = !string.IsNullOrEmpty(schemaRegistryUrl) ? schemaRegistryUrl : string.Empty;
        }

        public KafkaConsumerAttribute(string Topic, Type EventType) : this(Topic, EventType, null) { }
    }
}

