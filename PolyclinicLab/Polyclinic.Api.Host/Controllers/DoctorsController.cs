using Microsoft.AspNetCore.Mvc;
using Polyclinic.Domain.Models;
using Polyclinic.Application;

namespace Polyclinic.Api.Host.Controllers;

/// <summary>
/// Provides CRUD operations for managing doctors in the polyclinic.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DoctorController(DoctorService service) : ControllerBase
{
    /// <summary>
    /// Returns a list of all doctors.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll() => Ok(service.GetAll());

    /// <summary>
    /// Returns a specific doctor by their unique ID.
    /// </summary>
    /// <param name="id">The ID of the doctor to retrieve.</param>
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var doctor = service.Get(id);
        return doctor is null ? NotFound($"Doctor with Id = {id} was not found.") : Ok(doctor);
    }

    /// <summary>
    /// Creates a new doctor record.
    /// </summary>
    /// <param name="doc">The doctor information to create.</param>
    [HttpPost]
    public IActionResult Create([FromBody] Doctor doc)
    {
        service.Create(doc);
        return CreatedAtAction(nameof(Get), new { id = doc.Id }, doc);
    }

    /// <summary>
    /// Updates an existing doctor record by ID.
    /// </summary>
    /// <param name="id">The ID of the doctor to update.</param>
    /// <param name="doc">The updated doctor information.</param>
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Doctor doc)
    {
        var existing = service.Get(id);
        if (existing == null)
            return NotFound($"Doctor with Id = {id} was not found.");

        doc.Id = id;
        service.Update(doc);
        return Ok(doc);
    }

    /// <summary>
    /// Deletes a doctor record by ID.
    /// </summary>
    /// <param name="id">The ID of the doctor to delete.</param>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var existing = service.Get(id);
        if (existing == null)
            return NotFound($"Doctor with Id = {id} was not found.");

        service.Delete(id);
        return NoContent();
    }
}
