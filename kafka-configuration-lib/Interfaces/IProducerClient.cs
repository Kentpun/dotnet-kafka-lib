namespace KP.Lib.Kafka.Interfaces;

public interface IProducerClient
{
    void Produce(string topic, object message, TimeSpan timeout);
    void InitializeProducer();
}