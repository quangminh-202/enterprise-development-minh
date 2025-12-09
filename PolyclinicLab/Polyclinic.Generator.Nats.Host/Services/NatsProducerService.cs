using System.Text.Json;
using NATS.Client.Core;
using NATS.Client.JetStream.Models;
using NATS.Net;
using Polyclinic.Application.Contracts;

namespace Polyclinic.Generator.Nats.Host.Services;

/// <summary>
/// NATS producer service for sending <see cref="CreateUpdateAppointmentDto"/> contracts to a JetStream subject.
/// </summary>
public class NatsProducerService(
    IConfiguration configuration,
    INatsConnection connection,
    ILogger<NatsProducerService> logger) : IProducerService
{
    private const int MaxRetries = 5;
    private const int AckTimeoutSeconds = 5;
    
    private readonly string _streamName = configuration.GetSection("Nats")["StreamName"] 
        ?? throw new KeyNotFoundException("StreamName section of Nats is missing");
    private readonly string _rawSubject = configuration.GetSection("Nats")["RawSubject"] 
        ?? throw new KeyNotFoundException("RawSubject section of Nats is missing");
    private readonly string _validatedSubject = configuration.GetSection("Nats")["ValidatedSubject"] 
        ?? throw new KeyNotFoundException("ValidatedSubject section of Nats is missing");

    public async Task<BatchAckResponse> SendBatchAsync<T>(IList<T> batch)
    {
        var batchId = Guid.NewGuid();
        
        await EnsureConnectedAsync();
        await EnsureStreamExistsAsync();
        
        var replyInbox = CreateReplyInbox();
        var ackTask = ListenForAcknowledgmentAsync(batchId, replyInbox);
        
        await PublishBatchAsync(batchId, batch, replyInbox);
        
        return await WaitForAcknowledgmentAsync(batchId, ackTask);
    }

    private async Task EnsureConnectedAsync()
    {
        var retryDelay = TimeSpan.FromSeconds(1);
        
        for (var attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                await connection.ConnectAsync();
                return;
            }
            catch (Exception ex) when (attempt < MaxRetries)
            {
                logger.LogWarning(ex, 
                    "Failed to connect to NATS (attempt {Attempt}/{MaxRetries}). Retrying in {Delay}s...", 
                    attempt, MaxRetries, retryDelay.TotalSeconds);
                
                await Task.Delay(retryDelay);
                retryDelay = TimeSpan.FromSeconds(retryDelay.TotalSeconds * 2);
            }
        }
    }

    private async Task EnsureStreamExistsAsync()
    {
        var context = connection.CreateJetStreamContext();
        await context.CreateOrUpdateStreamAsync(
            new StreamConfig(_streamName, [_rawSubject, _validatedSubject]));
    }

    private static string CreateReplyInbox() => $"_INBOX.{Guid.NewGuid():N}";

    private Task<BatchAckResponse> ListenForAcknowledgmentAsync(Guid batchId, string replyInbox)
    {
        var tcs = new TaskCompletionSource<BatchAckResponse>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        _ = Task.Run(async () =>
        {
            await foreach (var msg in connection.SubscribeAsync<byte[]>(replyInbox))
            {
                if (msg.Data != null && TryProcessAcknowledgment(msg.Data, batchId, out var ack))
                {
                    tcs.TrySetResult(ack);
                    break;
                }
            }
        });

        return tcs.Task;
    }

    private bool TryProcessAcknowledgment(byte[] data, Guid expectedBatchId, out BatchAckResponse? ack)
    {
        try
        {
            var response = JsonSerializer.Deserialize<BatchAckResponse>(data);
            if (response?.BatchId == expectedBatchId)
            {
                ack = response;
                return true;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to deserialize acknowledgment");
        }

        ack = null;
        return false;
    }

    private async Task PublishBatchAsync<T>(Guid batchId, IList<T> batch, string replyInbox)
    {
        var payload = new BatchMessage<T> 
        { 
            BatchId = batchId, 
            Batch = [.. batch]
        };
        var serializedPayload = JsonSerializer.SerializeToUtf8Bytes(payload);
        
        await connection.PublishAsync(_rawSubject, serializedPayload, replyTo: replyInbox);
        
        logger.LogInformation("Sent batch {BatchId} ({Count} items) to {Subject}", 
            batchId, batch.Count, _rawSubject);
    }

    private async Task<BatchAckResponse> WaitForAcknowledgmentAsync(Guid batchId, Task<BatchAckResponse> ackTask)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(AckTimeoutSeconds));
        var completed = await Task.WhenAny(ackTask, Task.Delay(Timeout.Infinite, cts.Token));

        if (completed != ackTask)
        {
            logger.LogWarning("No ACK received for batch {BatchId} within {Timeout}s timeout", batchId, AckTimeoutSeconds);
            return new BatchAckResponse { BatchId = batchId };
        }

        return await ackTask;
    }
}