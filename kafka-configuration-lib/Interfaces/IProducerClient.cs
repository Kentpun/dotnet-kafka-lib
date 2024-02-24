namespace kafka_configuration_lib.Interfaces;

public interface IProducerClient
{
    void Produce(string topic, object message, TimeSpan timeout);
    void InitializeProducer();
}