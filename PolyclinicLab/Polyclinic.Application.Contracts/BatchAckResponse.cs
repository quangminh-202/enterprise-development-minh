namespace Polyclinic.Application.Contracts;

/// <summary>
/// Represents the acknowledgment response for a batch sent by the producer
/// </summary>
public class BatchAckResponse
{
    /// <summary>
    /// The unique identifier of the batch
    /// </summary>
    public Guid BatchId { get; set; }

    /// <summary>
    /// Items successfully inserted from the batch
    /// </summary>
    public List<object>? InsertedDtos { get; set; }

    /// <summary>
    /// The number of items successfully inserted from the batch
    /// </summary>
    public int Inserted => InsertedDtos?.Count ?? 0;

    /// <summary>
    /// Indicates whether the batch was successfully processed
    /// </summary>
    public bool Success => Inserted > 0;
}
