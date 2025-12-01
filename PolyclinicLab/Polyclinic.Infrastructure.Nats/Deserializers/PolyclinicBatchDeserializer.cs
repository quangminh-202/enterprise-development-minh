using NATS.Client.Core;
using Polyclinic.Application.Contracts;
using System.Buffers;
using System.Text.Json;

namespace Polyclinic.Infrastructure.Nats.Deserializers;

/// <summary>
/// Deserializer for batch messages containing a collection of contracts.
/// </summary>
/// <typeparam name="T">Type of contract in the batch.</typeparam>
public class PolyclinicBatchDeserializer<T> : INatsDeserialize<BatchMessage<T>>
{
    public BatchMessage<T>? Deserialize(in ReadOnlySequence<byte> buffer)
    {
        var reader = new Utf8JsonReader(buffer);
        return JsonSerializer.Deserialize<BatchMessage<T>>(ref reader);
    }
}
