using System;
using System.Reflection;
using Confluent.Kafka;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using kafka_configuration_lib.Configurations;

namespace kafka_configuration_lib
{
	public class KafkaConsumerClientFactory
	{
        private readonly IServiceProvider _serviceProvider;

        public KafkaConsumerClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public KafkaConsumerClient CreateClient(string groupId, KafkaConsumerConfig consumerConfig)
        {
            var client = ActivatorUtilities.CreateInstance<KafkaConsumerClient>(_serviceProvider, groupId, _serviceProvider, consumerConfig);
            var methods = GetMethodsAnnotatedWithKafkaConsumerAttribute();
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<KafkaConsumerAttribute>();
                client.RegisterMethod(attribute.Topic, method);
            }
            return client;
        }

        private IEnumerable<MethodInfo> GetMethodsAnnotatedWithKafkaConsumerAttribute()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var methods = assembly.GetTypes()
                .SelectMany(type => type.GetMethods())
                .Where(method => method.GetCustomAttributes(typeof(KafkaConsumerAttribute), false).Length > 0);
            return methods;
        }
    }
}

