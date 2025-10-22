using Microsoft.AspNetCore.Mvc;
using Polyclinic.Domain.Models;
using Polyclinic.Application;

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
    /// <param name="a">The appointment data to create.</param>
    [HttpPost]
    public IActionResult Create([FromBody] Appointment a)
    {
        service.Create(a);
        return CreatedAtAction(nameof(Get), new { id = a.Id }, a);
    }

    /// <summary>
    /// Updates an existing appointment by its ID.
    /// </summary>
    /// <param name="id">The ID of the appointment to update.</param>
    /// <param name="a">The updated appointment data.</param>
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Appointment a)
    {
        var existing = service.Get(id);
        if (existing == null)
            return NotFound($"Appointment with Id = {id} was not found.");

        a.Id = id;
        service.Update(a);
        return Ok(a);
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
