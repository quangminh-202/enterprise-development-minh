using Microsoft.AspNetCore.Mvc;
using Polyclinic.Generator.Nats.Host.Interfaces;
using Polyclinic.Generator.Nats.Host.Models;
using Polyclinic.Generator.Nats.Host.Services;

namespace Polyclinic.Generator.Nats.Host.Controllers;

/// <summary>
/// Controller for generating contracts and sending them via NATS
/// </summary>
/// <param name="logger">Logger for tracking operations</param>
/// <param name="producerService">Service for sending messages to NATS</param>
[Route("api/[controller]")]
[ApiController]
public class GeneratorController(
    ILogger<GeneratorController> logger,
    IProducerService producerService) : ControllerBase
{

    /// <summary>
    /// Generates and sends contracts in batches with ACK-based retry logic
    /// </summary>
    /// <param name="batchSize">Number of contracts per batch (default: 10)</param>
    /// <param name="payloadLimit">Total number of contracts to generate (default: 100)</param>
    /// <param name="waitTime">Delay in seconds between batches (default: 2)</param>
    /// <returns>Summary of generated contracts</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GenerationSummary), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<GenerationSummary>> Generate(
        [FromQuery] int batchSize = 10,
        [FromQuery] int payloadLimit = 100,
        [FromQuery] int waitTime = 2)
    {
        var startTime = DateTime.UtcNow;
        logger.LogInformation(
            "Generating {Limit} contracts via {BatchSize} batches with {WaitTime}s delay",
            payloadLimit, batchSize, waitTime);

        try
        {
            var patientCount = 0;
            var doctorCount = 0;
            var appointmentCount = 0;
            var batchIds = new List<Guid>();
            var counter = 0;

            while (counter < payloadLimit)
            {
                // Generate and send patients
                var patients = ContractGenerator.GeneratePatients(batchSize);
                var patientResult = await SendWithRetryAsync(
                    patients,
                    producerService.SendPatientsAsync,
                    "patients");
                
                patientCount += patientResult.Inserted;
                batchIds.Add(patientResult.BatchId);

                // Generate and send doctors
                var doctors = ContractGenerator.GenerateDoctors(batchSize);
                var doctorResult = await SendWithRetryAsync(
                    doctors,
                    producerService.SendDoctorsAsync,
                    "doctors");
                
                doctorCount += doctorResult.Inserted;
                batchIds.Add(doctorResult.BatchId);

                // Generate and send appointments
                var appointments = ContractGenerator.GenerateAppointments(batchSize);
                var appointmentResult = await SendWithRetryAsync(
                    appointments,
                    producerService.SendAppointmentsAsync,
                    "appointments");
                
                appointmentCount += appointmentResult.Inserted;
                batchIds.Add(appointmentResult.BatchId);

                counter += batchSize;

                if (counter < payloadLimit)
                    await Task.Delay(waitTime * 1000);
            }

            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;

            logger.LogInformation(
                "Finished sending {Total} messages in {Duration}s with {Batch} messages per batch",
                patientCount + doctorCount + appointmentCount, duration.TotalSeconds, batchSize);

            var summary = new GenerationSummary(
                TotalGenerated: patientCount + doctorCount + appointmentCount,
                PatientsGenerated: patientCount,
                DoctorsGenerated: doctorCount,
                AppointmentsGenerated: appointmentCount,
                StartTime: startTime,
                EndTime: endTime,
                Duration: duration,
                BatchIds: batchIds,
                TotalBatches: batchIds.Count
            );

            return Ok(summary);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception happened during {Method} method of {Controller}",
                nameof(Generate), GetType().Name);
            return StatusCode(500, $"{ex.Message}\n\r{ex.InnerException?.Message}");
        }
    }

    /// <summary>
    /// Send batch with ACK-based retry logic for partial failures
    /// </summary>
    private async Task<Polyclinic.Application.Contracts.BatchAckResponse> SendWithRetryAsync<T>(
        IList<T> batch,
        Func<IList<T>, Task<Polyclinic.Application.Contracts.BatchAckResponse>> sendFunc,
        string typeName)
    {
        var remaining = batch.Count;
        var batchOffset = 0;
        var totalInserted = 0;
        Guid lastBatchId = Guid.Empty;

        while (remaining > 0)
        {
            var currentBatch = batch.Skip(batchOffset).Take(remaining).ToList();
            var result = await sendFunc(currentBatch);
            lastBatchId = result.BatchId;

            if (!result.Success)
            {
                logger.LogWarning(
                    "Batch {BatchId} of {Type} failed. Retrying {Remaining} items...",
                    result.BatchId, typeName, remaining);
                
                // Wait before retry
                await Task.Delay(1000);
                continue;
            }

            var inserted = result.Inserted;
            totalInserted += inserted;
            remaining -= inserted;
            batchOffset += inserted;

            if (remaining > 0)
            {
                logger.LogWarning(
                    "Batch {BatchId}: {Remaining} {Type} not inserted, retrying...", 
                    result.BatchId, remaining, typeName);
            }
            else
            {
                logger.LogInformation(
                    "Batch {BatchId}: Successfully inserted {Inserted} {Type}",
                    result.BatchId, inserted, typeName);
            }
        }

        return new Polyclinic.Application.Contracts.BatchAckResponse 
        { 
            BatchId = lastBatchId,
            InsertedDtos = new List<object>() // Simplified - don't track individual items
        };
    }
}
