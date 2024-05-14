using Confluent.Kafka;

namespace KP.Lib.Kafka.Helpers;

public static class KafkaAdminHelper
{
    public static IEnumerable<TopicPartition> GetTopicPartition(string topic, string bootstrapServers)
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
}