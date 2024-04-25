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

			this.ConsumerConfig.SaslMechanism = kafkaOptions.UseSasl ? Enum.Parse<SaslMechanism>(kafkaOptions.SaslMechanism) : null;
            this.ConsumerConfig.SecurityProtocol = kafkaOptions.UseSasl ? Enum.Parse<SecurityProtocol>(kafkaOptions.SecurityProtocol) : null;
            this.ConsumerConfig.SaslUsername = kafkaOptions.UseSasl ? kafkaOptions.SaslUsername : "";
            this.ConsumerConfig.SaslPassword = kafkaOptions.UseSasl ? kafkaOptions.SaslPassword : "";
        }
	}
}

