using System;
using kafka_configuration_lib;
using kafka_configuration_lib.Examples;

namespace test_kafka
{
	public class TestConsumer
	{
        [KafkaConsumer("test-topic", typeof(TestClass))]
        public void HandleMessage(TestClass message)
        {
            Console.WriteLine($"1: Received message: {message.id}: {message.message}");
            // Your processing logic here
        }

        [KafkaConsumer("test-topic-2", typeof(TestClass))]
        public void HandleMessage2(TestClass message)
        {
            Console.WriteLine($"2: Received message2: {message.id}: {message.message}");
            // Your processing logic here
        }
    }
}

