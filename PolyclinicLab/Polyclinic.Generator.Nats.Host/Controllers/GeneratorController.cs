using Microsoft.AspNetCore.Mvc;
using Polyclinic.Application.Contracts;
using Polyclinic.Generator.Nats.Host.Services;
using Polyclinic.Generator.Nats.Host.Generator;

namespace Polyclinic.Generator.Nats.Host.Controllers;

/// <summary>
/// Controller for generating appointment contracts and sending them via the message bus.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class GeneratorController(ILogger<GeneratorController> logger, IProducerService producerService) : ControllerBase
{
    /// <summary>
    /// Generates and sends a specified number of appointment contracts in batches with a delay between batches.
    /// </summary>
    /// <param name="batchSize">Number of contracts per batch.</param>
    /// <param name="payloadLimit">Total number of contracts to generate and send.</param>
    /// <param name="waitTime">Delay in seconds between sending each batch.</param>
    /// <returns>List of generated <see cref="CreateUpdateAppointmentDto"/> objects.</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<CreateUpdateAppointmentDto>>> Get([FromQuery] int batchSize = 10, [FromQuery] int payloadLimit = 100, [FromQuery] int waitTime = 2)
    {
        logger.LogInformation("Starting generation: {limit} appointments, batch size: {batchSize}", payloadLimit, batchSize);
        try
        {
            var list = new List<CreateUpdateAppointmentDto>(payloadLimit);
            var counter = 0;
            while (counter < payloadLimit)
            {
                var batch = AppointmentGenerator.GenerateAppointments(batchSize);
                var remaining = batch.Count;
                var successfulItems = 0;

                while (remaining > 0)
                {
                    var currentBatch = batch.Skip(successfulItems).Take(remaining).ToList();
                    var result = await producerService.SendBatchAsync(currentBatch);

                    if (!result.Success)
                    {
                        logger.LogWarning("Batch failed, regenerating {remaining} items", remaining);
                        var newAppointments = AppointmentGenerator.GenerateAppointments(remaining);
                        batch = [.. batch.Take(successfulItems), .. newAppointments];
                        continue;
                    }

                    var inserted = result.Inserted;
                    remaining -= inserted;
                    successfulItems += inserted;

                    list.AddRange(currentBatch.Take(inserted));

                    if (remaining > 0)
                        logger.LogWarning("{remaining} items not inserted, retrying", remaining);
                }

                counter += batchSize;
                await Task.Delay(waitTime * 1000);
            }
            logger.LogInformation("Generation completed: {total} appointments sent", list.Count);
            return Ok(list);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during appointment generation");
            return StatusCode(500, $"{ex.Message}\n\r{ex.InnerException?.Message}");
        }
    }}