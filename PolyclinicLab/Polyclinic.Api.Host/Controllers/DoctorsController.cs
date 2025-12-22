using Microsoft.AspNetCore.Mvc;
using Polyclinic.Application.Contracts;
using Polyclinic.Application.Interfaces;

namespace Polyclinic.Api.Host.Controllers;

/// <summary>
/// Provides CRUD operations for managing doctors in the polyclinic.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DoctorsController(IDoctorService service, ILogger<DoctorsController> logger) : ControllerBase
{
    /// <summary>
    /// Returns a list of all doctors.
    /// </summary>
    [HttpGet]
    public ActionResult<List<DoctorDto>> GetAll()
    {
        try
        {
            var doctors = service.GetAll();
            logger.LogInformation("Retrieved {Count} doctors", doctors.Count);
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting all doctors");
            return StatusCode(500, "An error occurred while retrieving doctors");
        }
    }

    /// <summary>
    /// Returns a specific doctor by their unique ID.
    /// </summary>
    /// <param name="id">The ID of the doctor to retrieve.</param>
    [HttpGet("{id}")]
    public ActionResult<DoctorDto> Get(int id)
    {
        try
        {
            if (id <= 0)
            {
                logger.LogWarning("Invalid doctor ID provided: {Id}", id);
                return BadRequest("Doctor ID must be greater than 0");
            }
            
            var doctor = service.Get(id);
            if (doctor == null)
            {
                logger.LogWarning("Doctor with ID {Id} was not found", id);
                return NotFound($"Doctor with Id = {id} was not found.");
            }
            
            return Ok(doctor);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting doctor with ID {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the doctor");
        }
    }

    /// <summary>
    /// Creates a new doctor record.
    /// </summary>
    /// <param name="dto">The doctor information to create.</param>
    [HttpPost]
    public ActionResult<DoctorDto> Create([FromBody] CreateUpdateDoctorDto dto)
    {
        try
        {
            if (dto == null)
            {
                logger.LogWarning("Create doctor called with null DTO");
                return BadRequest("Doctor data is required");
            }

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                logger.LogWarning("Create doctor called with empty FullName");
                return BadRequest("Full name is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Specialization))
            {
                logger.LogWarning("Create doctor called with empty Specialization");
                return BadRequest("Specialization is required");
            }

            if (dto.Experience < 0)
            {
                logger.LogWarning("Create doctor called with negative Experience: {Experience}", dto.Experience);
                return BadRequest("Experience cannot be negative");
            }

            var createdDoctor = service.Create(dto);
            logger.LogInformation("Successfully created doctor {Id}", createdDoctor.Id);
            return Ok(createdDoctor);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation error while creating doctor");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating doctor");
            return StatusCode(500, "An error occurred while creating the doctor");
        }
    }

    /// <summary>
    /// Updates an existing doctor record by ID.
    /// </summary>
    /// <param name="id">The ID of the doctor to update.</param>
    /// <param name="dto">The updated doctor information.</param>
    [HttpPut("{id}")]
    public ActionResult<DoctorDto> Update(int id, [FromBody] CreateUpdateDoctorDto dto)
    {
        try
        {
            if (id <= 0)
            {
                logger.LogWarning("Update doctor called with invalid ID: {Id}", id);
                return BadRequest("Doctor ID must be greater than 0");
            }

            if (dto == null)
            {
                logger.LogWarning("Update doctor called with null DTO for ID: {Id}", id);
                return BadRequest("Doctor data is required");
            }

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                logger.LogWarning("Update doctor called with empty FullName for ID: {Id}", id);
                return BadRequest("Full name is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Specialization))
            {
                logger.LogWarning("Update doctor called with empty Specialization for ID: {Id}", id);
                return BadRequest("Specialization is required");
            }

            if (dto.Experience < 0)
            {
                logger.LogWarning("Update doctor called with negative Experience: {Experience} for ID: {Id}", dto.Experience, id);
                return BadRequest("Experience cannot be negative");
            }

            var updatedDoctor = service.Update(id, dto);
            logger.LogInformation("Successfully updated doctor {Id}", id);
            return Ok(updatedDoctor);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation error while updating doctor {Id}", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating doctor {Id}", id);
            return StatusCode(500, "An error occurred while updating the doctor");
        }
    }

    /// <summary>
    /// Deletes a doctor record by ID.
    /// Also deletes all appointments associated with this doctor.
    /// </summary>
    /// <param name="id">The ID of the doctor to delete.</param>
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        try
        {
            if (id <= 0)
            {
                logger.LogWarning("Delete doctor called with invalid ID: {Id}", id);
                return BadRequest("Doctor ID must be greater than 0");
            }

            var deleted = service.Delete(id);
            if (!deleted)
            {
                logger.LogWarning("Delete doctor called for non-existent ID: {Id}", id);
                return NotFound($"Doctor with Id = {id} was not found.");
            }

            logger.LogInformation("Successfully deleted doctor {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting doctor {Id}", id);
            return StatusCode(500, "An error occurred while deleting the doctor");
        }
    }
}
