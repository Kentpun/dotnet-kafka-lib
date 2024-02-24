using kafka_configuration_lib.Configurations;
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

    public KafkaConsumerHostedService(
        KafkaConsumerClientFactory consumerClientFactory, 
        KafkaOptions options, 
        ILogger<KafkaConsumerHostedService> logger,
        List<string> topics,
        Type eventType)
    {
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
        _kafkaOptions = options;
        _eventType = eventType;
        _consumerConfig = new KafkaConsumerConfig(options);
        _consumerClientFactory = consumerClientFactory;
        _consumerClient = _consumerClientFactory.CreateClient(_kafkaOptions, _eventType);
        _topics = topics;
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
                _consumerClient.Listening(TimeSpan.FromMilliseconds(1000), cancellationToken);
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