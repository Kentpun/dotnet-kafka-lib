using System;
using Confluent.Kafka;
namespace kafka_configuration_lib.Configurations
{
	public class KafkaConsumerConfig
	{
		private string _bootstrapServerEndpoints;
		private string _consumerGroupdId;
		public ConsumerConfig ConsumerConfig;

		public KafkaConsumerConfig(string bootstrapServerEndpoints, string consumerGroupId)
		{
			this._bootstrapServerEndpoints = bootstrapServerEndpoints;
			this._consumerGroupdId = consumerGroupId;
			this.ConsumerConfig = new ConsumerConfig
			{
				BootstrapServers = _bootstrapServerEndpoints,
				GroupId = _consumerGroupdId,
				AutoOffsetReset = AutoOffsetReset.Earliest
			};

        }
	}
}

