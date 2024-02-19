using System;
using System.Reflection;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace kafka_configuration_lib.Helpers
{
	public class KafkaConsumerRegistration : IKafkaConsumerRegistration
	{
		private readonly IConsumer<Ignore, string> _kafkaConsumer;

		public KafkaConsumerRegistration(IConsumer<Ignore, string> kafkaConsumer)
		{
			_kafkaConsumer = kafkaConsumer;
		}

        public void RegisterKafkaConsumers()
        {
            var types = Assembly.GetCallingAssembly().GetTypes();

            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                  .Where(m => m.GetCustomAttributes(typeof(KafkaConsumerAttribute), true).Length > 0);

                foreach (var method in methods)
                {
                    var kafkaConsumerAttribute = method.GetCustomAttribute<KafkaConsumerAttribute>();
                    if (kafkaConsumerAttribute != null)
                    {
                        var topic = kafkaConsumerAttribute.Topic;
                        _kafkaConsumer.Subscribe(topic);

                        Console.WriteLine($"Registered Kafka consumer: {type.Name}.{method.Name}");
                    }
                }
            }
        }

    }
}

