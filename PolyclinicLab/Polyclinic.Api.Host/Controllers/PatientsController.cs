using Microsoft.AspNetCore.Mvc;
using Polyclinic.Application.Contracts;
using Polyclinic.Application.Interfaces;

namespace Polyclinic.Api.Host.Controllers;

/// <summary>
/// Provides CRUD operations for managing patients in the polyclinic.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PatientController(IPatientService service, ILogger<PatientController> logger) : ControllerBase
{
    /// <summary>
    /// Returns a list of all patients.
    /// </summary>
    [HttpGet]
    public ActionResult<List<PatientDto>> GetAll()
    {
        try
        {
            var patients = service.GetAll();
            logger.LogInformation("Retrieved {Count} patients", patients.Count);
            return Ok(patients);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting all patients");
            return StatusCode(500, "An error occurred while retrieving patients");
        }
    }

    /// <summary>
    /// Returns a specific patient by their unique ID.
    /// </summary>
    /// <param name="id">The ID of the patient to retrieve.</param>
    [HttpGet("{id}")]
    public ActionResult<PatientDto> Get(int id)
    {
        try
        {
            if (id <= 0)
            {
                logger.LogWarning("Invalid patient ID provided: {Id}", id);
                return BadRequest("Patient ID must be greater than 0");
            }

            var patient = service.Get(id);
            if (patient == null)
            {
                logger.LogWarning("Patient with ID {Id} was not found", id);
                return NotFound($"Patient with Id = {id} was not found.");
            }
            
            return Ok(patient);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting patient with ID {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the patient");
        }
    }

    /// <summary>
    /// Creates a new patient record.
    /// </summary>
    /// <param name="dto">The patient information to create.</param>
    [HttpPost]
    public ActionResult<PatientDto> Create([FromBody] CreateUpdatePatientDto dto)
    {
        try
        {
            if (dto == null)
            {
                logger.LogWarning("Create patient called with null DTO");
                return BadRequest("Patient data is required");
            }

            var createdPatient = service.Create(dto);
            logger.LogInformation("Successfully created patient {Id}", createdPatient.Id);
            return CreatedAtAction(nameof(Get), new { id = createdPatient.Id }, createdPatient);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating patient");
            return StatusCode(500, "An error occurred while creating the patient");
        }
    }

    /// <summary>
    /// Updates an existing patient record by ID.
    /// </summary>
    /// <param name="id">The ID of the patient to update.</param>
    /// <param name="dto">The updated patient information.</param>
    [HttpPut("{id}")]
    public ActionResult<PatientDto> Update(int id, [FromBody] CreateUpdatePatientDto dto)
    {
        try
        {
            if (id <= 0)
            {
                logger.LogWarning("Update patient called with invalid ID: {Id}", id);
                return BadRequest("Patient ID must be greater than 0");
            }

            if (dto == null)
            {
                logger.LogWarning("Update patient called with null DTO for ID: {Id}", id);
                return BadRequest("Patient data is required");
            }

            var updatedPatient = service.Update(id, dto);
            logger.LogInformation("Successfully updated patient {Id}", id);
            return Ok(updatedPatient);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation error while updating patient {Id}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating patient {Id}", id);
            return StatusCode(500, "An error occurred while updating the patient");
        }
    }

    /// <summary>
    /// Deletes a patient record by ID.
    /// </summary>
    /// <param name="id">The ID of the patient to delete.</param>
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        try
        {
            if (id <= 0)
            {
                logger.LogWarning("Delete patient called with invalid ID: {Id}", id);
                return BadRequest("Patient ID must be greater than 0");
            }

            var deleted = service.Delete(id);
            if (!deleted)
            {
                logger.LogWarning("Delete patient called for non-existent ID: {Id}", id);
                return NotFound($"Patient with Id = {id} was not found.");
            }

            logger.LogInformation("Successfully deleted patient {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting patient {Id}", id);
            return StatusCode(500, "An error occurred while deleting the patient");
        }
    }
}
