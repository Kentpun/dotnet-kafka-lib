using System;
using kafka_configuration_lib.Configurations;
using Confluent.Kafka;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace kafka_configuration_lib.Helpers
{
	public static class KafkaServiceExtensions
	{
        public static IServiceCollection AddKafkaConsumers(this IServiceCollection services,
            KafkaConsumerConfig configureConsumer)
        {
            services.AddSingleton<IConsumer<Ignore, string>>(sp =>
            {
                return new ConsumerBuilder<Ignore, string>(configureConsumer.ConsumerConfig).Build();
            });

            services.AddSingleton<IKafkaConsumerRegistration, KafkaConsumerRegistration>();

            return services;
        }

    }
}

