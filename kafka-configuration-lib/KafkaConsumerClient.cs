using System;
using System.Reflection;
using System.Text;
using Avro;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KP.Lib.Kafka.Configurations;
using KP.Lib.Kafka.Helpers;
using KP.Lib.Kafka.Interfaces;

namespace KP.Lib.Kafka
{
    public class KafkaConsumerClient : IConsumerClient
    {
        private static readonly SemaphoreSlim ConnectionLock = new(1, 1);
        private readonly KafkaOptions _options;
        private readonly IServiceProvider _serviceProvider;
        private Dictionary<string, MethodInfo> _topicMethods;
        private Dictionary<string, object> _topicMethodInstances;
        private readonly KafkaConsumerConfig _consumerConfig;
        private readonly Type _eventType;
        private readonly SchemaRegistryConfig? _schemaRegistryConfig;
        private readonly string _schemaRegistryUrl;

        private IConsumer<string, byte[]> _consumer;

        public KafkaConsumerClient(KafkaOptions options, IServiceProvider serviceProvider, Type? eventType, string? schemaRegistryUrl)
        {
            _options = options;
            _consumerConfig = new KafkaConsumerConfig(options);
            _topicMethods = new Dictionary<string, MethodInfo>();
            _topicMethodInstances = new Dictionary<string, object>();
            _serviceProvider = serviceProvider;
            _eventType = eventType != null ? eventType : typeof(Object);
            _schemaRegistryConfig = !string.IsNullOrEmpty(schemaRegistryUrl) ? new SchemaRegistryConfig { Url = schemaRegistryUrl } : null;
        }

        public ICollection<string> FetchTopics(IEnumerable<string> topicNames)
        {
            return topicNames.ToList();
        }

        public IEnumerable<TopicPartition> GetTopicPartition(string topic, string bootstrapServers)
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = bootstrapServers
            };
            using (var adminClient = new AdminClientBuilder(config).Build())
            {
                var metadata = adminClient.GetMetadata(topic, TimeSpan.FromSeconds(10));
                if (metadata.Topics.FirstOrDefault(t => t.Topic.Equals(topic)) is TopicMetadata topicMetadata)
                {
                    return topicMetadata.Partitions.Select(p => new TopicPartition(topic, p.PartitionId));
                }
            }

            return Enumerable.Empty<TopicPartition>();
        }

        private void ConsumerClient_OnConsumeError(IConsumer<string, byte[]> consumer, Error e)
        {
            Console.WriteLine(e.Code + ": "+ e.Reason);
        }

        protected virtual IConsumer<string, byte[]> BuildConsumer(ConsumerConfig config)
        {
            //if (_schemaRegistryConfig != null)
            //{
            //    using (var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfig))
            //    {
            //        return new ConsumerBuilder<string, byte[]>(config)
            //            .SetValueDeserializer(new AvroDeserializer<byte[]>(schemaRegistry).AsSyncOverAsync())
            //            .SetErrorHandler(ConsumerClient_OnConsumeError)
            //            .Build();
            //    }

            //}
            return new ConsumerBuilder<string, byte[]>(config)
                .SetErrorHandler(ConsumerClient_OnConsumeError)
                .Build();
        }

        public void Connect()
        {
            if (_consumer != null) return;

            ConnectionLock.Wait();

            try
            {
                if (_consumer == null)
                {
                    var config = new ConsumerConfig();
                    config.BootstrapServers ??= _consumerConfig.ConsumerConfig.BootstrapServers;
                    config.GroupId ??= _options.ConsumerGroupId;
                    config.AutoOffsetReset ??= AutoOffsetReset.Earliest;
                    config.AllowAutoCreateTopics ??= true;
                    config.EnableAutoCommit ??= false;
                    config.LogConnectionClose ??= false;

                    _consumer = (IConsumer<string, byte[]>?) BuildConsumer(config);
                }
            }
            finally
            {
                ConnectionLock.Release();
            }
        }


        public void Subscribe(IEnumerable<string> topics)
        {
            // Subscribe logic
            if (topics == null) throw new ArgumentNullException(nameof(topics));
            
            _consumer.Subscribe(topics);
        }

        public void Listening(TimeSpan timeout, CancellationToken cancellationToken)
        {
            Connect();

            // Listening logic
            while (!cancellationToken.IsCancellationRequested)
            {
                ConsumeResult<string, byte[]> consumerResult;

                try
                {
                    var consumeResult = _consumer.Consume(timeout);
                    
                    if (consumeResult != null && consumeResult.Message != null)
                    {
                        var messageTypeHeader = consumeResult.Message.Headers
                            .FirstOrDefault(h => h.Key == "MessageType");
                        
                        if (messageTypeHeader != null)
                        {
                            var messageType = Encoding.UTF8.GetString(messageTypeHeader.GetValueBytes());
                            var topic = consumeResult.Topic;

                            // Check if the message type matches what you expect
                            var message = consumeResult.Message.Value;
                            if (_eventType.Name == "Byte[]")
                            {
                                if (_topicMethods.TryGetValue(topic, out MethodInfo method) &&
                                _topicMethodInstances.TryGetValue(topic, out object instance))
                                {
                                    var parameters = new object[] { message };
                                    method.Invoke(instance, parameters);
                                }
                            } else if (_eventType.Name == "String")
                            {
                                // Process the message
                                // Console.WriteLine($"Message Content: {Encoding.UTF8.GetString(consumeResult.Message.Value)}");

                                string deserializedMessage = Encoding.UTF8.GetString(message);
                                if (_topicMethods.TryGetValue(topic, out MethodInfo method) &&
                                _topicMethodInstances.TryGetValue(topic, out object instance))
                                {
                                    var parameters = new object[] { deserializedMessage };
                                    method.Invoke(instance, parameters);
                                }
                            } else if (messageType == _eventType.Name || _eventType.Name == "Object")
                            {
                                // Process the message
                                // Console.WriteLine($"Message Content: {Encoding.UTF8.GetString(consumeResult.Message.Value)}");
                                
                                var deserializedMessage = KafkaEventConsumerHelper.DeserializeEvent(_eventType, message);
                                if (_topicMethods.TryGetValue(topic, out MethodInfo method) &&
                                _topicMethodInstances.TryGetValue(topic, out object instance))
                                {
                                    var parameters = new object[] { deserializedMessage };
                                    method.Invoke(instance, parameters);
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Unexpected message type: {messageType}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Message type not found in headers.");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Handle cancellation
                    continue;
                }
                catch (Exception ex)
                {
                    // Handle exception
                    Console.WriteLine($"Error consuming message: {ex.Message}");
                }
                
            }
        }

        public void Commit(object? sender)
        {
            // Commit logic
            _consumer.Commit((ConsumeResult<string, byte[]>)sender!);
        }

        public void RegisterMethod(string topic, MethodInfo method, object targetInstance)
        {
            _topicMethods[topic] = method;
            _topicMethodInstances[topic] = targetInstance;
        }

        public void InitializeConsumer()
        {
            _consumer = new ConsumerBuilder<string, byte[]>(_consumerConfig.ConsumerConfig)
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                .SetStatisticsHandler((_, json) => Console.WriteLine($"Statistics: {json}"))
                .Build();
        }

        public void Reject(object? sender)
        {
            _consumer!.Assign(_consumer.Assignment);
        }

        public void Dispose()
        {
            // Dispose logic
            _consumer.Close();
            _consumer.Dispose();
        }
    }
}

