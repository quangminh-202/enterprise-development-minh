using Microsoft.AspNetCore.Mvc;
using Polyclinic.Application.Contracts;
using Polyclinic.Application.Services;

namespace Polyclinic.Api.Host.Controllers;

/// <summary>
/// Provides CRUD operations for managing medical appointments in the polyclinic.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AppointmentController(AppointmentService service) : ControllerBase
{
    /// <summary>
    /// Returns a list of all appointments.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll() => Ok(service.GetAll());

    /// <summary>
    /// Returns a single appointment by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the appointment.</param>
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var appointment = service.Get(id);
        if (appointment == null)
            return NotFound($"Appointment with Id = {id} was not found.");
        return Ok(appointment);
    }

    /// <summary>
    /// Creates a new appointment record.
    /// </summary>
    /// <param name="dto">The appointment data to create.</param>
    [HttpPost]
    public IActionResult Create([FromBody] AppointmentDto dto)
    {
        service.Create(dto);
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    /// <summary>
    /// Updates an existing appointment by its ID.
    /// </summary>
    /// <param name="id">The ID of the appointment to update.</param>
    /// <param name="dto">The updated appointment data.</param>
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] AppointmentDto dto)
    {
        var existing = service.Get(id);
        if (existing == null)
            return NotFound($"Appointment with Id = {id} was not found.");

        var updatedDto = dto with { Id = id };
        service.Update(updatedDto);
        return Ok(updatedDto);
    }

    /// <summary>
    /// Deletes an appointment by its ID.
    /// </summary>
    /// <param name="id">The ID of the appointment to delete.</param>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var existing = service.Get(id);
        if (existing == null)
            return NotFound($"Appointment with Id = {id} was not found.");

        service.Delete(id);
        return NoContent();
    }
}
