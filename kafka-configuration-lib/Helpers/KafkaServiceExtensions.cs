using System;
using kafka_configuration_lib.Configurations;
using Confluent.Kafka;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using kafka_configuration_lib.Interfaces;
using kafka_configuration_lib.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace kafka_configuration_lib.Helpers
{
	public static class KafkaServiceExtensions
	{
        public static void UseKafkaConsumer(this IServiceCollection services, KafkaOptions kafkaOptions, KafkaConsumerConfig consumerConfig)
        {
            services.AddSingleton<KafkaConsumerClientFactory>();

            // Retrieve the registered KafkaConsumerClientFactory
            var serviceProvider = services.BuildServiceProvider();
            var clientFactory = serviceProvider.GetService<KafkaConsumerClientFactory>();

            // Get the assembly where the KafkaConsumerAttribute is defined
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            // Find all types in the assembly that have methods with the KafkaConsumerAttribute
            var typesWithConsumers = assembly.GetTypes()
                .Where(type => type.GetMethods()
                    .Any(method => method.GetCustomAttributes(typeof(KafkaConsumerAttribute), false).Length > 0))
                .ToList();

            // Register the annotated methods with the KafkaConsumerClient
            foreach (var type in typesWithConsumers)
            {
                var methods = type.GetMethods()
                    .Where(method => method.GetCustomAttributes(typeof(KafkaConsumerAttribute), false).Length > 0);

                foreach (var method in methods)
                {
                    var attribute = method.GetCustomAttribute<KafkaConsumerAttribute>();
                    services.AddHostedService<KafkaConsumerHostedService>(provider => new KafkaConsumerHostedService(
                        clientFactory,
                        kafkaOptions,
                        provider.GetRequiredService<ILogger<KafkaConsumerHostedService>>(),
                        new List<string>(){attribute.Topic}
                    ));
                }
            }
        }
    }
}

