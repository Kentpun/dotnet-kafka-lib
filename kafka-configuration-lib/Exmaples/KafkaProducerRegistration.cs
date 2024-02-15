using System;
using System.Reflection;
using kafka_configuration_lib.Configurations;
using Confluent.Kafka;

namespace kafka_configuration_lib.Examples
{
	public class KafkaProducerRegistration
	{
        private readonly KafkaProducerConfig _kafkaProducerConfig;

        public KafkaProducerRegistration(KafkaProducerConfig kafkaProducerConfig)
		{
            _kafkaProducerConfig = kafkaProducerConfig;
        }
        
        public void RegisterProducers(object instance)
        {
            Type type = instance.GetType();

            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                       .Where(m => m.GetCustomAttributes(typeof(KafkaProducerAttribute), false).Length > 0)
                                       .ToArray();

            using (var producer = new ProducerBuilder<string, string>(_kafkaProducerConfig.ProducerConfig).Build())
            {
                foreach (MethodInfo method in methods)
                {
                    KafkaProducerAttribute attribute = (KafkaProducerAttribute)method.GetCustomAttributes(typeof(KafkaProducerAttribute), false).FirstOrDefault();
                    string topic = attribute?.Topic;

                    if (!string.IsNullOrEmpty(topic))
                    {
                        Action<DeliveryReport<string, string>> handler = (DeliveryReport<string, string> report) =>
                        {
                            if (report.Error.Code == ErrorCode.NoError)
                            {
                                Console.WriteLine($"Produced message to topic {report.Topic}, partition {report.Partition}, offset {report.Offset}");
                            }
                            else
                            {
                                Console.WriteLine($"Failed to produce message: {report.Error.Reason}");
                            }
                        };

                        Action<string, Object> producerAction = (string key, Object value) =>
                        {
                            producer.Produce(topic, new Message<string, string> { Key = key, Value = value.ToString() }, handler);
                        };

                        // Invoke the method and pass the producer action
                        method.Invoke(instance, new object[] { producerAction });
                    }
                }

                // Wait for any in-flight messages to be delivered
                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}

