using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Confluent.Kafka;

namespace KP.Lib.Kafka.Helpers;

public class ObjectDeserializer<T> : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
        {
            return default(T);
        }

        try
        {
            using (MemoryStream stream = new MemoryStream(data.ToArray()))
            {
                IFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(stream);
            }
        }
        catch (Exception ex)
        {
            throw new SerializationException($"Error deserializing message: {ex.Message}", ex);
        }
    }
}
