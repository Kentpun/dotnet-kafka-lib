﻿using System;
using KP.Lib.Kafka.Configurations;
using Confluent.Kafka;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using KP.Lib.Kafka.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KP.Lib.Kafka.Helpers
{
	public static class KafkaServiceExtensions
	{
        public static void UseKafkaProducer(this IServiceCollection services)
        {
            services.AddSingleton<KafkaProducerClientFactory>();
        }
        
        public static void UseKafkaConsumer(this IServiceCollection services, KafkaOptions kafkaOptions)
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
                    var declaringType = serviceProvider.GetService(method.DeclaringType);
                    var attribute = method.GetCustomAttribute<KafkaConsumerAttribute>();
                    var eventType = attribute.EventType;
                    var schemaRegistryUrl = !string.IsNullOrEmpty(attribute.SchemaRegistryUrl) ? attribute.SchemaRegistryUrl : "";
                    Console.WriteLine($"Registering KafkaConsumer: Class: {declaringType.ToString()}; EventType: {eventType.Name}; MethodName: {method.Name}");
                    services.AddSingleton<IHostedService, KafkaConsumerHostedService>(provider => new KafkaConsumerHostedService(
                        clientFactory,
                        kafkaOptions,
                        provider.GetRequiredService<ILogger<KafkaConsumerHostedService>>(),
                        new List<string>() { attribute.Topic },
                        eventType,
                        method,
                        declaringType,
                        schemaRegistryUrl
                    ));
                    Console.WriteLine($"Registration Succeed: {declaringType.ToString()}.{method.Name}");
                }
            }
        }
    }
}

