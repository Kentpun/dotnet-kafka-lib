using System;
namespace KP.Lib.Kafka.Interfaces
{
	public interface IConsumerClient : IDisposable
	{
        /// <summary>
        /// Get topic identifiers
        /// </summary>
        /// <param name="topicNames">Names of the requested topics</param>
        /// <returns>Topic identifiers</returns>
        ICollection<string> FetchTopics(IEnumerable<string> topicNames)
        {
            return topicNames.ToList();
        }

        /// <summary>
        /// Subscribe to a set of topics to the message queue
        /// </summary>
        /// <param name="topics"></param>
        void Subscribe(IEnumerable<string> topics);

        /// <summary>
        /// Start listening
        /// </summary>
        void Listening(TimeSpan timeout, CancellationToken cancellationToken);

        /// <summary>
        /// Manual submit message offset when the message consumption is complete
        /// </summary>
        void Commit(object? sender);

    }
}

