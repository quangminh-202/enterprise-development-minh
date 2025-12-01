using System.Text.Json;
using NATS.Client.Core;
using NATS.Client.JetStream.Models;
using NATS.Net;
using Polyclinic.Application.Contracts;

namespace Polyclinic.Generator.Nats.Host.Services;

/// <summary>
/// NATS producer service for sending <see cref="CreateUpdateAppointmentDto"/> contracts to a JetStream subject.
/// </summary>
/// <param name="configuration">Application configuration.</param>
/// <param name="connection">Connection to NATS server.</param>
/// <param name="logger">Logger for information and errors.</param>
public class NatsProducerService(
    IConfiguration configuration,
    INatsConnection connection,
    ILogger<NatsProducerService> logger) : IProducerService
{
    private readonly string _streamName = configuration.GetSection("Nats")["StreamName"] 
        ?? throw new KeyNotFoundException("StreamName section of Nats is missing");
    private readonly string _rawSubject = configuration.GetSection("Nats")["RawSubject"] 
        ?? throw new KeyNotFoundException("RawSubject section of Nats is missing");

    public async Task<BatchAckResponse> SendAppointmentsAsync<T>(IList<T> batch)
    {
        var batchId = Guid.NewGuid();
        var payload = new
        {
            BatchId = batchId,
            Data = batch
        };

        await connection.ConnectAsync();
        var context = connection.CreateJetStreamContext();
        await context.CreateOrUpdateStreamAsync(new StreamConfig(_streamName, [_rawSubject]));

        var replyInbox = $"_INBOX.{Guid.NewGuid():N}";
        var tcs = new TaskCompletionSource<BatchAckResponse>(TaskCreationOptions.RunContinuationsAsynchronously);

        _ = Task.Run(async () =>
        {
            await foreach (var msg in connection.SubscribeAsync<byte[]>(replyInbox))
            {
                try
                {
                    var ack = JsonSerializer.Deserialize<BatchAckResponse>(msg.Data);
                    if (ack is not null && ack.BatchId == batchId)
                    {
                        tcs.TrySetResult(new BatchAckResponse { BatchId = batchId, InsertedDtos = ack.InsertedDtos });
                        break;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to deserialize ack on inbox {inbox}", replyInbox);
                }
            }
        });

        await connection.PublishAsync(_rawSubject, JsonSerializer.SerializeToUtf8Bytes(payload), replyTo: replyInbox);
        logger.LogInformation("Sent batch {batchId} ({count} items) to {subject}", batchId, batch.Count, _rawSubject);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var completed = await Task.WhenAny(tcs.Task, Task.Delay(Timeout.Infinite, cts.Token));

        if (completed != tcs.Task)
        {
            logger.LogWarning("No ACK received for batch {batchId} within timeout", batchId);
            return new BatchAckResponse { BatchId = batchId };
        }

        return await tcs.Task;
    }
}