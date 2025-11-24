namespace Polyclinic.Application.Contracts;

/// <summary>
/// Represents a batch message containing a collection of contracts
/// </summary>
/// <typeparam name="T">Type of contract in the batch</typeparam>
public class BatchMessage<T>
{
    /// <summary>
    /// The unique identifier of the batch
    /// </summary>
    public Guid BatchId { get; set; }

    /// <summary>
    /// The collection of contracts in this batch
    /// </summary>
    public List<T>? Data { get; set; }
}
