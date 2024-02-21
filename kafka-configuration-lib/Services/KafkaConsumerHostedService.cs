using kafka_configuration_lib.Configurations;
using kafka_configuration_lib.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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

    public KafkaConsumerHostedService(
        KafkaConsumerClientFactory consumerClientFactory, 
        KafkaOptions options, 
        ILogger<KafkaConsumerHostedService> logger,
        IEnumerable<string> topics)
    {
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
        _consumerConfig = new KafkaConsumerConfig(options);
        _consumerClientFactory = consumerClientFactory;
        _consumerClient = _consumerClientFactory.CreateClient(_consumerConfig);
        _topics = topics;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            _consumerClient.Subscribe(_topics);
            while (!cancellationToken.IsCancellationRequested)
            {
                _consumerClient.Listening(TimeSpan.FromMilliseconds(1000), cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in ExecuteAsync: {ex.Message}");
        }
        finally
        {
            _consumerClient.Dispose();
        }
    }
}