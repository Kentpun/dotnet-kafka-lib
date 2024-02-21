using System;
using Confluent.Kafka;

namespace kafka_configuration_lib.Configurations
{
	public class KafkaProducerConfig
	{
        public string BootstrapServerEndpoints { get; set; }
        public string ClientId { get; set; }
        public ProducerConfig ProducerConfig;

        public KafkaProducerConfig(string bootstrapServerEndpoints, string clientId)
		{
            this.BootstrapServerEndpoints = bootstrapServerEndpoints;
            this.ClientId = clientId;
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
        }
	}
}

