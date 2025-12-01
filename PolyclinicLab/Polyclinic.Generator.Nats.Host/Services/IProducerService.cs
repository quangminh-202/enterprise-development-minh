namespace Polyclinic.Generator.Nats.Host.Services;

/// <summary>
/// Interface for producer service that sends batches of data.
/// </summary>
public interface IProducerService
{
    /// <summary>
    /// Sends a batch of appointments asynchronously.
    /// </summary>
    /// <typeparam name="T">Type of items in the batch.</typeparam>
    /// <param name="batch">List of items to send.</param>
    /// <returns>Batch acknowledgment response.</returns>
    Task<BatchAckResponse> SendAppointmentsAsync<T>(IList<T> batch);
}

/// <summary>
/// Response from batch processing.
/// </summary>
public class BatchAckResponse
{
    public Guid BatchId { get; set; }
    public bool Success => Inserted > 0;
    public int Inserted => InsertedDtos?.Count ?? 0;
    public IList<object>? InsertedDtos { get; set; }
}
