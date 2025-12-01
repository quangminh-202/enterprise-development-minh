using Polyclinic.Application.Contracts;
using Polyclinic.Application.Interfaces;
using Polyclinic.Infrastructure.Nats.Deserializers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;
using System.Text.Json;

namespace Polyclinic.Infrastructure.Nats;

/// <summary>
/// Background service consuming Appointment contracts from a NATS JetStream subject.
/// </summary>
public class PolyclinicNatsConsumer(
    INatsConnection connection,
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<PolyclinicNatsConsumer> logger) : BackgroundService
{
    private readonly string _streamName = configuration.GetSection("Nats")["StreamName"] 
        ?? throw new KeyNotFoundException("StreamName section of Nats is missing");
    private readonly string _validatedSubject = configuration.GetSection("Nats")["ValidatedSubject"] 
        ?? throw new KeyNotFoundException("ValidatedSubject section of Nats is missing");

    /// <summary>
    /// Starts the background service, sets up JetStream consumer, and begins processing appointment messages.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token to stop the service.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await connection.ConnectAsync();
            var context = new NatsJSContext(connection);

            try
            {
                var streamConfig = new StreamConfig(_streamName, [_validatedSubject]);
                await context.CreateStreamAsync(streamConfig, stoppingToken);
                logger.LogInformation("Created or verified stream {Stream}", _streamName);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Stream {Stream} might already exist", _streamName);
            }

            var consumer = await context.CreateConsumerAsync(_streamName,
                new ConsumerConfig("appointment-consumer")
                {
                    DeliverPolicy = ConsumerConfigDeliverPolicy.New,
                    AckPolicy = ConsumerConfigAckPolicy.Explicit,
                    FilterSubject = _validatedSubject
                },
                stoppingToken);

            logger.LogInformation("Created consumer for stream {Stream}", _streamName);

            await ConsumeAppointments(consumer, stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred during receiving contracts from NATS");
        }
    }

    /// <summary>
    /// Consumes appointment batches from NATS, saves them to database, and sends acknowledgments.
    /// Handles partial failures by continuing to process remaining appointments in the batch.
    /// </summary>
    /// <param name="consumer">JetStream consumer for receiving messages.</param>
    /// <param name="stoppingToken">Cancellation token to stop consuming.</param>
    private async Task ConsumeAppointments(INatsJSConsumer consumer, CancellationToken stoppingToken)
    {
        await foreach (var message in consumer.ConsumeAsync(
            new PolyclinicBatchDeserializer<CreateUpdateAppointmentDto>(), 
            cancellationToken: stoppingToken))
        {
            var batchMsg = message.Data;
            if (batchMsg?.Data is null)
            {
                await message.AckAsync(cancellationToken: stoppingToken);
                continue;
            }

            var insertedDtos = new List<object>();
            using var scope = scopeFactory.CreateScope();
            var appointmentService = scope.ServiceProvider.GetRequiredService<IAppointmentService>();

            foreach (var appointment in batchMsg.Data)
            {
                try
                {
                    var created = appointmentService.Create(appointment);
                    insertedDtos.Add(appointment);
                    logger.LogInformation("Saved Appointment: {Id} - {Date}", created.Id, created.Date);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error saving appointment for patient {PatientId}", appointment.PatientId);
                }
            }

            await SendAckAsync(message.ReplyTo, new BatchAckResponse
            {
                BatchId = batchMsg.BatchId,
                InsertedDtos = insertedDtos
            });

            await message.AckAsync(cancellationToken: stoppingToken);
            logger.LogInformation("Processed appointment batch {BatchId}: {Inserted}/{Total}", 
                batchMsg.BatchId, insertedDtos.Count, batchMsg.Data.Count);
        }
    }

    /// <summary>
    /// Sends acknowledgment response back to the producer with batch processing results.
    /// </summary>
    /// <param name="replyTo">NATS reply inbox address.</param>
    /// <param name="ack">Acknowledgment containing batch ID and successfully inserted items.</param>
    private async Task SendAckAsync(string? replyTo, BatchAckResponse ack)
    {
        if (string.IsNullOrEmpty(replyTo)) return;

        try
        {
            var payload = JsonSerializer.SerializeToUtf8Bytes(ack);
            await connection.PublishAsync(replyTo, payload);
            logger.LogDebug("Sent ACK for batch {BatchId} to {ReplyTo}", ack.BatchId, replyTo);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send ACK to {ReplyTo}", replyTo);
        }
    }
}
