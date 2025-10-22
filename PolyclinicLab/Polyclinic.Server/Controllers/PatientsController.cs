using Microsoft.AspNetCore.Mvc;
using Polyclinic.Domain.Models;
using Polyclinic.Application;

namespace Polyclinic.Api.Host.Controllers;

/// <summary>
/// Provides CRUD operations for managing patients in the polyclinic.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PatientController(PatientService service) : ControllerBase
{
    /// <summary>
    /// Returns a list of all patients.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll() => Ok(service.GetAll());

    /// <summary>
    /// Returns a specific patient by their unique ID.
    /// </summary>
    /// <param name="id">The ID of the patient to retrieve.</param>
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var patient = service.Get(id);
        return patient is null ? NotFound($"Patient with Id = {id} was not found.") : Ok(patient);
    }

    /// <summary>
    /// Creates a new patient record.
    /// </summary>
    /// <param name="p">The patient information to create.</param>
    [HttpPost]
    public IActionResult Create([FromBody] Patient p)
    {
        service.Create(p);
        return CreatedAtAction(nameof(Get), new { id = p.Id }, p);
    }

    /// <summary>
    /// Updates an existing patient record by ID.
    /// </summary>
    /// <param name="id">The ID of the patient to update.</param>
    /// <param name="p">The updated patient information.</param>
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Patient p)
    {
        var existing = service.Get(id);
        if (existing == null)
            return NotFound($"Patient with Id = {id} was not found.");

        p.Id = id;
        service.Update(p);
        return Ok(p);
    }

    /// <summary>
    /// Deletes a patient record by ID.
    /// </summary>
    /// <param name="id">The ID of the patient to delete.</param>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var existing = service.Get(id);
        if (existing == null)
            return NotFound($"Patient with Id = {id} was not found.");

        service.Delete(id);
        return NoContent();
    }
}
