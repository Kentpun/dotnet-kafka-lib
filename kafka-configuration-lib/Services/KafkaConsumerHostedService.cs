using System.Reflection;
using kafka_configuration_lib.Configurations;
using kafka_configuration_lib.Examples;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace kafka_configuration_lib.Services;


public class KafkaConsumerHostedService : BackgroundService
{
    private readonly KafkaOptions _kafkaOptions;
    private readonly KafkaConsumerConfig _consumerConfig;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly ILogger<KafkaConsumerHostedService> _logger;
    private readonly KafkaConsumerClient _consumerClient;
    private readonly KafkaConsumerClientFactory _consumerClientFactory;
    private readonly IEnumerable<string> _topics;
    private readonly Type _eventType;
    private readonly MethodInfo _methodInfo;
    private readonly Object _declaringType;

    public KafkaConsumerHostedService(
        KafkaConsumerClientFactory consumerClientFactory, 
        KafkaOptions options, 
        ILogger<KafkaConsumerHostedService> logger,
        List<string> topics,
        Type eventType,
        MethodInfo methodInfo,
        Object declaringType)
    {
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
        _kafkaOptions = options;
        _eventType = eventType;
        _consumerConfig = new KafkaConsumerConfig(options);
        _consumerClientFactory = consumerClientFactory;
        
        _topics = topics;
        _methodInfo = methodInfo;
        _declaringType = declaringType;
        _consumerClient = _consumerClientFactory.CreateClient(_methodInfo, _declaringType, _kafkaOptions, _eventType);
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(_consumerConfig.ConsumerConfig.BootstrapServers);
            _logger.LogInformation(_consumerConfig.ConsumerConfig.GroupId);
            _logger.LogInformation(_topics.Count().ToString());
            _consumerClient.Subscribe(_topics);
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Run(() =>
                {
                    _consumerClient.Listening(TimeSpan.FromMilliseconds(1000), cancellationToken);
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in ExecuteAsync: {ex}");
        }
        finally
        {
            _consumerClient.Dispose();
        }
    }
}