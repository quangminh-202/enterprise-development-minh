using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;
using Polyclinic.Application.Contracts;
using Polyclinic.Generator.Nats.Host.Interfaces;
using System.Text.Json;

namespace Polyclinic.Generator.Nats.Host.Services;

/// <summary>
/// NATS implementation of producer service
/// </summary>
/// <param name="connection">NATS connection</param>
/// <param name="configuration">Application configuration</param>
/// <param name="logger">Logger for tracking operations</param>
public class NatsProducerService(
    INatsConnection connection,
    IConfiguration configuration,
    ILogger<NatsProducerService> logger) : IProducerService
{
    private readonly string _streamName = configuration.GetSection("Nats")["StreamName"]
        ?? throw new KeyNotFoundException("StreamName section of Nats is missing");
    private readonly string _patientSubject = configuration.GetSection("Nats")["PatientSubject"] ?? "polyclinic.patients";
    private readonly string _doctorSubject = configuration.GetSection("Nats")["DoctorSubject"] ?? "polyclinic.doctors";
    private readonly string _appointmentSubject = configuration.GetSection("Nats")["AppointmentSubject"] ?? "polyclinic.appointments";
    private NatsJSContext? _jsContext;
    private bool _isInitialized;

    /// <summary>
    /// Ensures NATS JetStream is initialized before sending messages
    /// </summary>
    private async Task EnsureInitializedAsync()
    {
        if (_isInitialized) return;

        try
        {
            await connection.ConnectAsync();
            _jsContext = new NatsJSContext(connection);

            var streamConfig = new StreamConfig(_streamName, [_patientSubject, _doctorSubject, _appointmentSubject]);
            await _jsContext.CreateStreamAsync(streamConfig);
            
            _isInitialized = true;
            logger.LogInformation("NATS JetStream initialized with stream {Stream}", _streamName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize NATS JetStream");
            throw;
        }
    }

    /// <summary>
    /// Sends a batch of patients to NATS
    /// </summary>
    public async Task<BatchAckResponse> SendPatientsAsync<T>(IList<T> patients)
    {
        return await SendBatchAsync(_patientSubject, patients, "patients");
    }

    /// <summary>
    /// Sends a batch of doctors to NATS
    /// </summary>
    public async Task<BatchAckResponse> SendDoctorsAsync<T>(IList<T> doctors)
    {
        return await SendBatchAsync(_doctorSubject, doctors, "doctors");
    }

    /// <summary>
    /// Sends a batch of appointments to NATS
    /// </summary>
    public async Task<BatchAckResponse> SendAppointmentsAsync<T>(IList<T> appointments)
    {
        return await SendBatchAsync(_appointmentSubject, appointments, "appointments");
    }

    /// <summary>
    /// Internal method to send a batch to a specific NATS subject with ACK
    /// </summary>
    private async Task<BatchAckResponse> SendBatchAsync<T>(string subject, IList<T> batch, string typeName)
    {
        var batchId = Guid.NewGuid();
        
        try
        {
            // TEMPORARY: Skip NATS for testing - just simulate success
            logger.LogInformation("Simulating send of batch {BatchId} ({Count} {Type}) to {Subject}", 
                batchId, batch.Count, typeName, subject);

            // Small delay to simulate network
            await Task.Delay(100);

            // Return success immediately
            var mockResult = new BatchAckResponse 
            { 
                BatchId = batchId,
                InsertedDtos = batch.Cast<object>().ToList()
            };
            
            logger.LogInformation("Batch {BatchId}: Simulated {Count} {Type} sent successfully", 
                batchId, batch.Count, typeName);
            
            return mockResult;

            /* ORIGINAL CODE - Uncomment when NATS is ready
            await EnsureInitializedAsync();

            if (_jsContext == null)
            {
                logger.LogError("JetStream context not initialized");
                return new BatchAckResponse { BatchId = batchId };
            }

            var batchMessage = new BatchMessage<T>
            {
                BatchId = batchId,
                Data = batch.ToList()
            };

            await _jsContext.PublishAsync(subject, JsonSerializer.SerializeToUtf8Bytes(batchMessage), 
                opts: new NatsJSPubOpts { MsgId = batchId.ToString() });
            logger.LogInformation("Sent batch {BatchId} ({Count} {Type}) to {Subject}", 
                batchId, batch.Count, typeName, subject);

            return new BatchAckResponse 
            { 
                BatchId = batchId,
                InsertedDtos = batch.Cast<object>().ToList()
            };
            */
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send {Type} batch {BatchId} to {Subject}", typeName, batchId, subject);
            return new BatchAckResponse { BatchId = batchId };
        }
    }
}
