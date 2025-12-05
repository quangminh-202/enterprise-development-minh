using Polyclinic.Application.Contracts;
using Polyclinic.Infrastructure.Mongo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using NATS.Client.JetStream.Models;
using NATS.Net;
using System.Text.Json;

namespace Polyclinic.Validator.Nats;

/// <summary>
/// Background service that validates incoming appointment batches from the raw NATS subject,
/// filters out invalid or duplicate appointments, and publishes valid appointments to the validated subject.
/// </summary>
/// <param name="connection">NATS connection.</param>
/// <param name="scopeFactory">Factory for creating service scopes.</param>
/// <param name="configuration">Application configuration.</param>
/// <param name="logger">Logger for informational and error messages.</param>
public class AppointmentValidatorService(
    INatsConnection connection,
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<AppointmentValidatorService> logger
) : BackgroundService
{
    private readonly string _streamName = configuration.GetSection("Nats")["StreamName"] ?? throw new KeyNotFoundException("StreamName section of Nats is missing");
    private readonly string _rawSubject = configuration.GetSection("Nats")["RawSubject"] ?? throw new KeyNotFoundException("RawSubject section of Nats is missing");
    private readonly string _validatedSubject = configuration.GetSection("Nats")["ValidatedSubject"] ?? throw new KeyNotFoundException("ValidatedSubject section of Nats is missing");

    /// <summary>
    /// Starts the background service and subscribes to the raw NATS subject to receive appointment batches.
    /// </summary>
    /// <param name="stoppingToken">Token to signal service shutdown.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await connection.ConnectAsync();
        var context = connection.CreateJetStreamContext();
        await context.CreateOrUpdateStreamAsync(new StreamConfig(_streamName, [_validatedSubject]), stoppingToken);

        logger.LogInformation("AppointmentValidatorService started, subscribing to {subject}", _rawSubject);

        await foreach (var msg in connection.SubscribeAsync<byte[]>(_rawSubject, cancellationToken: stoppingToken))
        {
            _ = ProcessMessageAsync(msg, stoppingToken);
        }
    }

    /// <summary>
    /// Processes a single NATS message containing a batch of appointments.
    /// </summary>
    /// <param name="msg">The raw NATS message containing the batch.</param>
    /// <param name="ct">Cancellation token to stop processing.</param>
    private async Task ProcessMessageAsync(NatsMsg<byte[]> msg, CancellationToken ct)
    {
        BatchMessage<CreateUpdateAppointmentDto>? batchMsg;
        try
        {
            batchMsg = JsonSerializer.Deserialize<BatchMessage<CreateUpdateAppointmentDto>>(msg.Data);
            if (batchMsg is null || batchMsg.Data is null)
            {
                logger.LogWarning("Malformed batch on {subject}", _rawSubject);
                await SendAck(msg.ReplyTo, new BatchAckResponse { BatchId = batchMsg?.BatchId ?? Guid.Empty });
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to deserialize batch from {subject}", _rawSubject);
            await SendAck(msg.ReplyTo, new BatchAckResponse { BatchId = Guid.Empty });
            return;
        }

        try
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PolyclinicDbContext>();

            var incoming = batchMsg.Data;
            
            // Extract unique IDs for validation
            var patientIds = incoming.Select(a => a.PatientId).Distinct().ToList();
            var doctorIds = incoming.Select(a => a.DoctorId).Distinct().ToList();
            var appointmentPairs = incoming
                .Select(a => new { a.DoctorId, a.Date })
                .Distinct()
                .ToList();

            // Check existing appointments (doctor can't have multiple appointments at same time)
            var existingAppointments = await context.Appointments
                .Where(a => doctorIds.Contains(a.DoctorId))
                .Select(a => new { a.DoctorId, a.Date })
                .ToListAsync(ct);

            var existingSet = new HashSet<string>(
                existingAppointments.Select(a => $"{a.DoctorId}:{a.Date:yyyy-MM-dd HH:mm}")
            );

            // Check if patients and doctors exist
            var existingPatients = await context.Patients
                .Where(p => patientIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync(ct);
            
            var existingDoctors = await context.Doctors
                .Where(d => doctorIds.Contains(d.Id))
                .Select(d => d.Id)
                .ToListAsync(ct);

            var patientSet = existingPatients.ToHashSet();
            var doctorSet = existingDoctors.ToHashSet();

            var validated = new List<CreateUpdateAppointmentDto>();
            foreach (var a in incoming)
            {
                if (!patientSet.Contains(a.PatientId))
                {
                    logger.LogDebug("Drop appointment: patient {PatientId} not found", a.PatientId);
                    continue;
                }
                if (!doctorSet.Contains(a.DoctorId))
                {
                    logger.LogDebug("Drop appointment: doctor {DoctorId} not found", a.DoctorId);
                    continue;
                }

                var key = $"{a.DoctorId}:{a.Date:yyyy-MM-dd HH:mm}";
                if (existingSet.Contains(key))
                {
                    logger.LogDebug("Drop appointment: doctor already has appointment at {Key}", key);
                    continue;
                }

                existingSet.Add(key);
                validated.Add(a);
            }

            if (validated.Count == 0)
            {
                logger.LogInformation("Batch {batchId} contains 0 valid appointments — replying 0", batchMsg.BatchId);
                await SendAck(msg.ReplyTo, new BatchAckResponse { BatchId = batchMsg.BatchId });
                return;
            }

            var outMsg = new BatchMessage<CreateUpdateAppointmentDto> { BatchId = batchMsg.BatchId, Data = validated };
            var payload = JsonSerializer.SerializeToUtf8Bytes(outMsg);

            await connection.PublishAsync(_validatedSubject, payload, replyTo: msg.ReplyTo, cancellationToken: ct);
            logger.LogInformation("Published validated batch {batchId} with {count} appointments to {subject}", batchMsg.BatchId, validated.Count, _validatedSubject);
            
            // Send ACK back to Generator immediately after validation
            await SendAck(msg.ReplyTo, new BatchAckResponse 
            { 
                BatchId = batchMsg.BatchId,
                InsertedDtos = validated.Cast<object>().ToList()
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error validating batch {batchId}", batchMsg.BatchId);
            await SendAck(msg.ReplyTo, new BatchAckResponse { BatchId = batchMsg.BatchId });
        }
    }

    /// <summary>
    /// Sends acknowledgment back to the producer indicating processing result of the batch.
    /// </summary>
    /// <param name="replyTo">The NATS inbox to send the acknowledgment to.</param>
    /// <param name="ack">The acknowledgment object containing the batch ID and inserted count.</param>
    private async Task SendAck(string? replyTo, BatchAckResponse ack)
    {
        if (string.IsNullOrEmpty(replyTo)) return;

        try
        {
            var payload = JsonSerializer.SerializeToUtf8Bytes(ack);
            await connection.PublishAsync(replyTo, payload);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send ack to {replyTo}", replyTo);
        }
    }
}
