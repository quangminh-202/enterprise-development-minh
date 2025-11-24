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

namespace Polyclinic.Infrastructure.Nats;

/// <summary>
/// Service for reading data from NATS subject using push consumer
/// </summary>
public class PolyclinicNatsConsumer : BackgroundService
{
    private readonly INatsConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PolyclinicNatsConsumer> _logger;
    private readonly string _streamName;
    private readonly string _patientSubject;
    private readonly string _doctorSubject;
    private readonly string _appointmentSubject;

    public PolyclinicNatsConsumer(
        INatsConnection connection,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<PolyclinicNatsConsumer> logger)
    {
        _connection = connection;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;

        _streamName = configuration.GetSection("Nats")["StreamName"] 
            ?? throw new KeyNotFoundException("StreamName section of Nats is missing");
        _patientSubject = configuration.GetSection("Nats")["PatientSubject"] 
            ?? "polyclinic.patients";
        _doctorSubject = configuration.GetSection("Nats")["DoctorSubject"] 
            ?? "polyclinic.doctors";
        _appointmentSubject = configuration.GetSection("Nats")["AppointmentSubject"] 
            ?? "polyclinic.appointments";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _connection.ConnectAsync();
            var js = new NatsJSContext(_connection);
            var context = js;

            // Create consumers for each subject
            var patientConsumer = await context.CreateConsumerAsync(_streamName,
                new ConsumerConfig
                {
                    DeliverPolicy = ConsumerConfigDeliverPolicy.All,
                    AckPolicy = ConsumerConfigAckPolicy.Explicit,
                    FilterSubject = _patientSubject
                },
                stoppingToken);

            var doctorConsumer = await context.CreateConsumerAsync(_streamName,
                new ConsumerConfig
                {
                    DeliverPolicy = ConsumerConfigDeliverPolicy.All,
                    AckPolicy = ConsumerConfigAckPolicy.Explicit,
                    FilterSubject = _doctorSubject
                },
                stoppingToken);

            var appointmentConsumer = await context.CreateConsumerAsync(_streamName,
                new ConsumerConfig
                {
                    DeliverPolicy = ConsumerConfigDeliverPolicy.All,
                    AckPolicy = ConsumerConfigAckPolicy.Explicit,
                    FilterSubject = _appointmentSubject
                },
                stoppingToken);

            _logger.LogInformation("Created consumers for stream {Stream}", _streamName);

            // Start consuming in parallel
            var tasks = new[]
            {
                Task.Run(() => ConsumePatients(patientConsumer, stoppingToken), stoppingToken),
                Task.Run(() => ConsumeDoctors(doctorConsumer, stoppingToken), stoppingToken),
                Task.Run(() => ConsumeAppointments(appointmentConsumer, stoppingToken), stoppingToken)
            };

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during receiving contracts from NATS");
        }
    }

    private async Task ConsumePatients(INatsJSConsumer consumer, CancellationToken stoppingToken)
    {
        await foreach (var message in consumer.ConsumeAsync(new PolyclinicBatchDeserializer<CreateUpdatePatientDto>(), cancellationToken: stoppingToken))
        {
            var batchMsg = message.Data;
            if (batchMsg?.Data is null)
            {
                await message.AckAsync(cancellationToken: stoppingToken);
                continue;
            }

            var insertedDtos = new List<object>();
            using var scope = _scopeFactory.CreateScope();
            var patientService = scope.ServiceProvider.GetRequiredService<IPatientService>();

            foreach (var patient in batchMsg.Data)
            {
                try
                {
                    var created = patientService.Create(patient);
                    insertedDtos.Add(patient);
                    _logger.LogInformation("Saved Patient: {Id} - {FullName}", created.Id, created.FullName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving patient: {FullName}", patient.FullName);
                }
            }

            // Send ACK back to producer
            await SendAckAsync(message.ReplyTo, new BatchAckResponse
            {
                BatchId = batchMsg.BatchId,
                InsertedDtos = insertedDtos
            });

            await message.AckAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("Processed patient batch {BatchId}: {Inserted}/{Total}", 
                batchMsg.BatchId, insertedDtos.Count, batchMsg.Data.Count);
        }
    }

    private async Task ConsumeDoctors(INatsJSConsumer consumer, CancellationToken stoppingToken)
    {
        await foreach (var message in consumer.ConsumeAsync(new PolyclinicBatchDeserializer<CreateUpdateDoctorDto>(), cancellationToken: stoppingToken))
        {
            var batchMsg = message.Data;
            if (batchMsg?.Data is null)
            {
                await message.AckAsync(cancellationToken: stoppingToken);
                continue;
            }

            var insertedDtos = new List<object>();
            using var scope = _scopeFactory.CreateScope();
            var doctorService = scope.ServiceProvider.GetRequiredService<IDoctorService>();

            foreach (var doctor in batchMsg.Data)
            {
                try
                {
                    var created = doctorService.Create(doctor);
                    insertedDtos.Add(doctor);
                    _logger.LogInformation("Saved Doctor: {Id} - {FullName}", created.Id, created.FullName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving doctor: {FullName}", doctor.FullName);
                }
            }

            // Send ACK back to producer
            await SendAckAsync(message.ReplyTo, new BatchAckResponse
            {
                BatchId = batchMsg.BatchId,
                InsertedDtos = insertedDtos
            });

            await message.AckAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("Processed doctor batch {BatchId}: {Inserted}/{Total}", 
                batchMsg.BatchId, insertedDtos.Count, batchMsg.Data.Count);
        }
    }

    private async Task ConsumeAppointments(INatsJSConsumer consumer, CancellationToken stoppingToken)
    {
        await foreach (var message in consumer.ConsumeAsync(new PolyclinicBatchDeserializer<CreateUpdateAppointmentDto>(), cancellationToken: stoppingToken))
        {
            var batchMsg = message.Data;
            if (batchMsg?.Data is null)
            {
                await message.AckAsync(cancellationToken: stoppingToken);
                continue;
            }

            var insertedDtos = new List<object>();
            using var scope = _scopeFactory.CreateScope();
            var appointmentService = scope.ServiceProvider.GetRequiredService<IAppointmentService>();

            foreach (var appointment in batchMsg.Data)
            {
                try
                {
                    var created = appointmentService.Create(appointment);
                    insertedDtos.Add(appointment);
                    _logger.LogInformation("Saved Appointment: {Id} - {Date}", created.Id, created.Date);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving appointment for patient {PatientId}", appointment.PatientId);
                }
            }

            // Send ACK back to producer
            await SendAckAsync(message.ReplyTo, new BatchAckResponse
            {
                BatchId = batchMsg.BatchId,
                InsertedDtos = insertedDtos
            });

            await message.AckAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("Processed appointment batch {BatchId}: {Inserted}/{Total}", 
                batchMsg.BatchId, insertedDtos.Count, batchMsg.Data.Count);
        }
    }

    /// <summary>
    /// Sends an acknowledgment response to the specified NATS inbox
    /// </summary>
    private async Task SendAckAsync(string? replyTo, BatchAckResponse ack)
    {
        if (string.IsNullOrEmpty(replyTo)) return;

        try
        {
            var payload = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(ack);
            await _connection.PublishAsync(replyTo, payload);
            _logger.LogDebug("Sent ACK for batch {BatchId} to {ReplyTo}", ack.BatchId, replyTo);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send ACK to {ReplyTo}", replyTo);
        }
    }
}
