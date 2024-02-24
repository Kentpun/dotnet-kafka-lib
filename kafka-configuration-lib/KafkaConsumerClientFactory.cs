using System;
using System.Reflection;
using Confluent.Kafka;
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

        public KafkaConsumerClient CreateClient(KafkaOptions options, Type eventType)
        {
            var client = new KafkaConsumerClient(options, _serviceProvider, eventType);
            var items = GetInstancesWithMethodAttribute(_serviceProvider);
            foreach (var item in items)
            {
                var instance = item.Item1;
                var method = item.Item2;
                Console.WriteLine(instance);
                Console.WriteLine(method);
                var attribute = method.GetCustomAttribute<KafkaConsumerAttribute>();
                client.RegisterMethod(attribute.Topic, method, instance);
                client.InitializeConsumer();
            }
            return client;
        }

        public IEnumerable<KafkaConsumerClient> CreateClients(KafkaOptions options)
        {
            List<KafkaConsumerClient> kafkaConsumerClients = new List<KafkaConsumerClient>();
            var items = GetInstancesWithMethodAttribute(_serviceProvider);
            foreach (var item in items)
            {
                var instance = item.Item1;
                var method = item.Item2;
                var attribute = method.GetCustomAttribute<KafkaConsumerAttribute>();
                IEnumerable<TopicPartition> topicPartitions = KafkaAdminHelper.GetTopicPartition(attribute.Topic, options.BootstrapServers);
                for (int partition = 0; partition < topicPartitions.Count() || partition < options.MaxConsumers; partition++)
                {
                    var client = ActivatorUtilities.CreateInstance<KafkaConsumerClient>(_serviceProvider, options.ConsumerGroupId, _serviceProvider);
                    client.RegisterMethod(attribute.Topic, method, instance);
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
        
        private IEnumerable<Tuple<object, MethodInfo>> GetInstancesWithMethodAttribute(IServiceProvider serviceProvider)
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            var instancesWithMethods = assembly.GetTypes()
                .SelectMany(type => type.GetMethods())
                .Where(method => method.GetCustomAttributes(typeof(KafkaConsumerAttribute), false).Length > 0)
                .Select(method => Tuple.Create(serviceProvider.GetService(method.DeclaringType), method));

            return instancesWithMethods;
        }

    }
}

