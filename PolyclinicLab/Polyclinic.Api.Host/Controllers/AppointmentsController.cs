using Microsoft.AspNetCore.Mvc;
using Polyclinic.Application.Contracts;
using Polyclinic.Application.Interfaces;

namespace Polyclinic.Api.Host.Controllers;

/// <summary>
/// Provides CRUD operations for managing medical appointments in the polyclinic.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AppointmentController(IAppointmentService service, ILogger<AppointmentController> logger) : ControllerBase
{
    /// <summary>
    /// Returns a list of all appointments.
    /// </summary>
    [HttpGet]
    public ActionResult<List<AppointmentDto>> GetAll()
    {
        try
        {
            var appointments = service.GetAll();
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting all appointments");
            return StatusCode(500, "An error occurred while retrieving appointments");
        }
    }

    /// <summary>
    /// Returns a single appointment by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the appointment.</param>
    [HttpGet("{id}")]
    public ActionResult<AppointmentDto> Get(int id)
    {
        try
        {
            if (id <= 0)
            {
                logger.LogWarning("Invalid appointment ID provided: {Id}", id);
                return BadRequest("Appointment ID must be greater than 0");
            }

            var appointment = service.Get(id);
            if (appointment == null)
            {
                logger.LogWarning("Appointment with ID {Id} was not found", id);
                return NotFound($"Appointment with Id = {id} was not found.");
            }
            
            return Ok(appointment);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting appointment with ID {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the appointment");
        }
    }

    /// <summary>
    /// Creates a new appointment record.
    /// </summary>
    /// <param name="dto">The appointment data to create.</param>
    [HttpPost]
    public ActionResult<AppointmentDto> Create([FromBody] CreateUpdateAppointmentDto dto)
    {
        try
        {
            if (dto == null)
            {
                logger.LogWarning("Create appointment called with null DTO");
                return BadRequest("Appointment data is required");
            }

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Create appointment validation failed: {Errors}",
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }

            var createdAppointment = service.Create(dto);
            logger.LogInformation("Successfully created appointment {Id} for doctor {DoctorId} and patient {PatientId}", 
                createdAppointment.Id, dto.DoctorId, dto.PatientId);
            return CreatedAtAction(nameof(Get), new { id = createdAppointment.Id }, createdAppointment);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation error while creating appointment");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating appointment");
            return StatusCode(500, "An error occurred while creating the appointment");
        }
    }

    /// <summary>
    /// Updates an existing appointment by its ID.
    /// </summary>
    /// <param name="id">The ID of the appointment to update.</param>
    /// <param name="dto">The updated appointment data.</param>
    [HttpPut("{id}")]
    public ActionResult<AppointmentDto> Update(int id, [FromBody] CreateUpdateAppointmentDto dto)
    {
        try
        {
            if (id <= 0)
            {
                logger.LogWarning("Update appointment called with invalid ID: {Id}", id);
                return BadRequest("Appointment ID must be greater than 0");
            }

            if (dto == null)
            {
                logger.LogWarning("Update appointment called with null DTO for ID: {Id}", id);
                return BadRequest("Appointment data is required");
            }

            if (!ModelState.IsValid)
            {
                logger.LogWarning("Update appointment validation failed: {Errors} (Id: {Id})",
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)), id);
                return BadRequest(ModelState);
            }

            var updatedAppointment = service.Update(id, dto);
            logger.LogInformation("Successfully updated appointment {Id}", id);
            return Ok(updatedAppointment);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation error while updating appointment {Id}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating appointment {Id}", id);
            return StatusCode(500, "An error occurred while updating the appointment");
        }
    }

    /// <summary>
    /// Deletes an appointment by its ID.
    /// </summary>
    /// <param name="id">The ID of the appointment to delete.</param>
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        try
        {
            if (id <= 0)
            {
                logger.LogWarning("Delete appointment called with invalid ID: {Id}", id);
                return BadRequest("Appointment ID must be greater than 0");
            }

            var deleted = service.Delete(id);
            if (!deleted)
            {
                logger.LogWarning("Delete appointment called for non-existent ID: {Id}", id);
                return NotFound($"Appointment with Id = {id} was not found.");
            }

            logger.LogInformation("Successfully deleted appointment {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting appointment {Id}", id);
            return StatusCode(500, "An error occurred while deleting the appointment");
        }
    }
}
