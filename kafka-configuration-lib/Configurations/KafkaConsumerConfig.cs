using System;
using Confluent.Kafka;
namespace kafka_configuration_lib.Configurations
{
	public class KafkaConsumerConfig
	{
		public string BootstrapServerEndpoints { get; set; }
		public string ConsumerGroupId { get; set; }
		public ConsumerConfig ConsumerConfig;

		public KafkaConsumerConfig(KafkaOptions kafkaOptions)
		{
			this.BootstrapServerEndpoints = kafkaOptions.BootstrapServers;
			this.ConsumerGroupId = kafkaOptions.ConsumerGroupId;
			this.ConsumerConfig = new ConsumerConfig
			{
				BootstrapServers = BootstrapServerEndpoints,
                GroupId = ConsumerGroupId,
				AutoOffsetReset = AutoOffsetReset.Earliest,
				Debug = kafkaOptions.Debug
			};

			if (kafkaOptions.UseSasl)
			{
				this.ConsumerConfig.SaslMechanism = Enum.Parse<SaslMechanism>(kafkaOptions.SaslMechanism);
                this.ConsumerConfig.SecurityProtocol = Enum.Parse<SecurityProtocol>(kafkaOptions.SecurityProtocol);
                this.ConsumerConfig.SaslUsername = kafkaOptions.SaslUsername;
                this.ConsumerConfig.SaslPassword = kafkaOptions.SaslPassword;
			}

        }
	}
}

