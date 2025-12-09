using Polyclinic.Application.Contracts;

namespace Polyclinic.Generator.Nats.Host.Services;

/// <summary>
/// Interface for producer service that sends batches of data.
/// </summary>
public interface IProducerService
{
    /// <summary>
    /// Sends a batch of items asynchronously.
    /// </summary>
    /// <typeparam name="T">Type of items in the batch.</typeparam>
    /// <param name="batch">List of items to send.</param>
    /// <returns>Batch acknowledgment response.</returns>
    public Task<BatchAckResponse> SendBatchAsync<T>(IList<T> batch);
}
