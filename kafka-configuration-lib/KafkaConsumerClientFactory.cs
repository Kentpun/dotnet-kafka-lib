using System;
using System.Reflection;
using Confluent.Kafka;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using kafka_configuration_lib.Configurations;
using kafka_configuration_lib.Helpers;

namespace kafka_configuration_lib
{
	public class KafkaConsumerClientFactory
	{
        private readonly IServiceProvider _serviceProvider;

        public KafkaConsumerClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public KafkaConsumerClient CreateClient(KafkaOptions options, KafkaConsumerConfig consumerConfig)
        {
            var client = new KafkaConsumerClient(options, _serviceProvider, consumerConfig);
            var methods = GetMethodsAnnotatedWithKafkaConsumerAttribute();
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<KafkaConsumerAttribute>();
                client.RegisterMethod(attribute.Topic, method);
                client.InitializeConsumer();
            }
            return client;
        }

        public IEnumerable<KafkaConsumerClient> CreateClients(KafkaConsumerConfig consumerConfig)
        {
            List<KafkaConsumerClient> kafkaConsumerClients = new List<KafkaConsumerClient>();
            var methods = GetMethodsAnnotatedWithKafkaConsumerAttribute();
            foreach (var method in methods)
            {
                
                var attribute = method.GetCustomAttribute<KafkaConsumerAttribute>();
                IEnumerable<TopicPartition> topicPartitions = KafkaAdminHelper.GetTopicPartition(attribute.Topic, consumerConfig.BootstrapServerEndpoints);
                for (int partition = 0; partition < topicPartitions.Count(); partition++)
                {
                    var client = ActivatorUtilities.CreateInstance<KafkaConsumerClient>(_serviceProvider, consumerConfig.ConsumerGroupId, _serviceProvider, consumerConfig);
                    client.RegisterMethod(attribute.Topic, method);
                    kafkaConsumerClients.Add(client);
                }
                
            }
            return kafkaConsumerClients;
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

