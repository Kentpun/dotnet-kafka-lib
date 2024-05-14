namespace KP.Lib.Kafka.Configurations;

public class KafkaOptions
{
    public string BootstrapServers { get; set; }
    public int MaxConsumers { get; set; }
    public string ConsumerGroupId { get; set; }
    public string ClientId { get; set; }
    public string Debug { get; set; }
    
    public string Topic { get; set; }

    public bool UseSasl { get; set; }
    public string SaslMechanism { get; set; }
    public string SecurityProtocol { get; set; }
    public string SaslUsername { get; set; }
    public string SaslPassword { get; set; }
}