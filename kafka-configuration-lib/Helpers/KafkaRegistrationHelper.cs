using System;
using kafka_configuration_lib.Configurations;
using Confluent.Kafka;
using System.Reflection;

namespace kafka_configuration_lib.Helpers
{
	public static class KafkaRegistrationHelper
	{
        public static void RegisterKafkaConsumers(object consumerInstance, KafkaConsumerConfig config, CancellationToken cancellationToken)
        {
            var methods = consumerInstance.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);

            using (var consumer = new ConsumerBuilder<Ignore, string>(config.ConsumerConfig).Build())
            {
                foreach (var method in methods)
                {
                    var kafkaConsumerAttribute = method.GetCustomAttribute<KafkaConsumerAttribute>();
                    if (kafkaConsumerAttribute != null)
                    {
                        var topic = kafkaConsumerAttribute.Topic;
                        consumer.Subscribe(topic);

                        Task.Run(() =>
                        {
                            while (!cancellationToken.IsCancellationRequested)
                            {
                                var consumeResult = consumer.Consume(cancellationToken);
                                method.Invoke(consumerInstance, new object[] { consumeResult.Message.Value });
                            }
                        }, cancellationToken);
                    }
                }
            }
        }
    }
}

