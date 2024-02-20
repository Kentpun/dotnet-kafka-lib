using System;
using System.Reflection;
using System.Text;
using Confluent.Kafka;
using kafka_configuration_lib.Configurations;
using kafka_configuration_lib.Interfaces;

namespace kafka_configuration_lib
{
    public class KafkaConsumerClient : IConsumerClient
    {
        private static readonly SemaphoreSlim ConnectionLock = new(1, 1);
        private readonly string _groupId;
        private readonly KafkaOptions _kafkaOptions;
        private readonly IServiceProvider _serviceProvider;
        private Dictionary<string, MethodInfo> _topicMethods;
        private readonly KafkaConsumerConfig _consumerConfig;

        private IConsumer<string, byte[]> _consumer;

        public KafkaConsumerClient(string groupId, KafkaOptions options, IServiceProvider serviceProvider, KafkaConsumerConfig consumerConfig)
        {
            _consumerConfig = consumerConfig;
            _topicMethods = new Dictionary<string, MethodInfo>();
            _serviceProvider = serviceProvider;
            _kafkaOptions = options ?? throw new ArgumentNullException(nameof(options));
        }

        public ICollection<string> FetchTopics(IEnumerable<string> topicNames)
        {
            return topicNames.ToList();
        }

        private void ConsumerClient_OnConsumeError(IConsumer<string, byte[]> consumer, Error e)
        {
            Console.WriteLine(e.Code + ": "+ e.Reason);
        }

        protected virtual IConsumer<string, byte[]> BuildConsumer(ConsumerConfig config)
        {
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
                    var config = new ConsumerConfig(new Dictionary<string, string>(_kafkaOptions.MainConfig));
                    config.BootstrapServers ??= _kafkaOptions.Servers;
                    config.GroupId ??= _groupId;
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
                        var topic = consumeResult.Topic;
                        var message = consumeResult.Message.Value;
                        if (_topicMethods.TryGetValue(topic, out MethodInfo method))
                        {
                            var parameters = new object[] { message };
                            method.Invoke(null, parameters);
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

        public void RegisterMethod(string topic, MethodInfo method)
        {
            _topicMethods[topic] = method;
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

