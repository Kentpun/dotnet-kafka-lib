using System;
using Confluent.Kafka;
namespace kafka_configuration_lib.Configurations
{
	public class KafkaConsumerConfig
	{
		public string BootstrapServerEndpoints { get; set; }
		public string ConsumerGroupId { get; set; }
		public ConsumerConfig ConsumerConfig;

		public KafkaConsumerConfig(string bootstrapServerEndpoints, string consumerGroupId)
		{
			this.BootstrapServerEndpoints = bootstrapServerEndpoints;
			this.ConsumerGroupId = consumerGroupId;
			this.ConsumerConfig = new ConsumerConfig
			{
				BootstrapServers = BootstrapServerEndpoints,
				GroupId = ConsumerGroupId,
				AutoOffsetReset = AutoOffsetReset.Earliest
			};

        }
	}
}

