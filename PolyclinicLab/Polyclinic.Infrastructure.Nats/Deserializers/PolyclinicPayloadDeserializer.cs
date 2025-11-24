using Polyclinic.Application.Contracts;
using NATS.Client.Core;
using System.Buffers;
using System.Text.Json;

namespace Polyclinic.Infrastructure.Nats.Deserializers;

/// <summary>
/// Deserializer for NATS payload data
/// </summary>
internal class PolyclinicPayloadDeserializer<T> : INatsDeserialize<IList<T>>
{
    public IList<T>? Deserialize(in ReadOnlySequence<byte> buffer) =>
        JsonSerializer.Deserialize<IList<T>>(buffer.ToArray());
}
