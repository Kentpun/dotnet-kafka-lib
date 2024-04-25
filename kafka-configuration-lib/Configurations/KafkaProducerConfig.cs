using System;
using Confluent.Kafka;

namespace kafka_configuration_lib.Configurations
{
	public class KafkaProducerConfig
	{
        public string BootstrapServerEndpoints { get; set; }
        public string ClientId { get; set; }
        public ProducerConfig ProducerConfig;

        public KafkaProducerConfig(KafkaOptions kafkaOptions)
		{
            this.BootstrapServerEndpoints = kafkaOptions.BootstrapServers;
            this.ClientId = kafkaOptions.ClientId;
            this.ProducerConfig = new ProducerConfig
            {
                BootstrapServers = BootstrapServerEndpoints,
                EnableIdempotence = true,
                ClientId = ClientId,
                Acks = Acks.All,
                MessageTimeoutMs = 5000,
                CompressionType = CompressionType.Gzip,
                MaxInFlight = 5
            };

            this.ProducerConfig.SaslMechanism = kafkaOptions.UseSasl ? Enum.Parse<SaslMechanism>(kafkaOptions.SaslMechanism) : null;
            this.ProducerConfig.SecurityProtocol = kafkaOptions.UseSasl ? Enum.Parse<SecurityProtocol>(kafkaOptions.SecurityProtocol) : null;
            this.ProducerConfig.SaslUsername = kafkaOptions.UseSasl ? kafkaOptions.SaslUsername : "";
            this.ProducerConfig.SaslPassword = kafkaOptions.UseSasl ? kafkaOptions.SaslPassword : "";
        }
	}
}

