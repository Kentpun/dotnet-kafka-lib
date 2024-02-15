namespace kafka_configuration_lib
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class KafkaConsumerAttribute : Attribute
    {
        public string Topic { get; set; }
        public KafkaConsumerAttribute(string topic)
        {
            this.Topic = topic;
        }
    }
}

