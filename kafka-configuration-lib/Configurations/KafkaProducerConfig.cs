using System;
using Confluent.Kafka;

namespace kafka_configuration_lib.Configurations
{
	public class KafkaProducerConfig
	{
        private string _bootstrapServerEndpoints;
        private string _clientId;
        public ProducerConfig ProducerConfig;

        public KafkaProducerConfig(string bootstrapServerEndpoints, string clientId)
		{
            this._bootstrapServerEndpoints = bootstrapServerEndpoints;
            this._clientId = clientId;
            this.ProducerConfig = new ProducerConfig
            {
                BootstrapServers = _bootstrapServerEndpoints,
                EnableIdempotence = true,
                ClientId = _clientId,
                Acks = Acks.All,
                MessageTimeoutMs = 5000,
                CompressionType = CompressionType.Gzip,
                MaxInFlight = 5
            };
        }
	}
}

