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

        public KafkaConsumerClient CreateClient(MethodInfo methodInfo, Object instance, KafkaOptions options, Type eventType)
        {
            var client = new KafkaConsumerClient(options, _serviceProvider, eventType);
            // var item = GetInstanceWithMethodAttribute(methodInfo, _serviceProvider);
            //
            // var instance = item.Item1;
            // var method = item.Item2;
            Console.WriteLine(instance);
            Console.WriteLine(methodInfo);
            var attribute = methodInfo.GetCustomAttribute<KafkaConsumerAttribute>();
            client.RegisterMethod(attribute.Topic, methodInfo, instance);
            client.InitializeConsumer();
            return client;
        }

        public IEnumerable<KafkaConsumerClient> CreateClients(List<MethodInfo> methodInfos, KafkaOptions options)
        {
            List<KafkaConsumerClient> kafkaConsumerClients = new List<KafkaConsumerClient>();
            var items = GetInstancesWithMethodAttribute(methodInfos, _serviceProvider);
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
        
        private Tuple<object, MethodInfo> GetInstanceWithMethodAttribute(MethodInfo methodInfo, IServiceProvider serviceProvider)
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            var instanceWithMethod = assembly.GetTypes()
                .SelectMany(type => type.GetMethods())
                .Where(method => method.Name == methodInfo.Name && method.GetCustomAttributes(typeof(KafkaConsumerAttribute), false).Length > 0)
                .Select(method => Tuple.Create(serviceProvider.GetService(method.DeclaringType), method))
                .FirstOrDefault();

            return instanceWithMethod;
        }
        
        private IEnumerable<Tuple<object, MethodInfo>> GetInstancesWithMethodAttribute(List<MethodInfo> methodInfos, IServiceProvider serviceProvider)
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            var instancesWithMethods = assembly.GetTypes()
                .SelectMany(type => type.GetMethods())
                .Where(method => methodInfos.Any(info => method.Name == info.Name) && method.GetCustomAttributes(typeof(KafkaConsumerAttribute), false).Length > 0)
                .Select(method => Tuple.Create(serviceProvider.GetService(method.DeclaringType), method));

            return instancesWithMethods;
        }

    }
}

