namespace kafka_configuration_lib.Configurations;

public class KafkaOptions
{
    public string BootstrapServers { get; set; }
    public int MaxConsumers { get; set; }
    public string ConsumerGroupId { get; set; }
    public string ClientId { get; set; }
}