using Polyclinic.Domain.Models;

namespace Polyclinic.Infrastructure.InMemory;

/// <summary>
/// In-memory repository implementation for managing appointments.
/// Provides basic CRUD operations.
/// </summary>
public class AppointmentInMemoryRepository : IRepository<Appointment, int>
{
    /// <summary>
    /// Internal in-memory collection used to store appointment data.
    /// </summary>
    private readonly List<Appointment> _appointments = [];

    /// <summary>
    /// Adds a new appointment to memory.
    /// </summary>
    public void Create(Appointment entity)
    {
        entity.Id = _appointments.Count == 0 ? 1 : _appointments.Max(a => a.Id) + 1;
        _appointments.Add(entity);
    }

    /// <summary>
    /// Finds an appointment by ID.
    /// </summary>
    public Appointment? Read(int id) => _appointments.FirstOrDefault(a => a.Id == id);

    /// <summary>
    /// Returns all appointments.
    /// </summary>
    public List<Appointment> ReadAll() => [.. _appointments];

    /// <summary>
    /// Updates an existing appointment.
    /// </summary>
    public void Update(Appointment entity)
    {
        var existing = Read(entity.Id);
        if (existing != null)
        {
            _appointments.Remove(existing);
            _appointments.Add(entity);
        }
    }

    /// <summary>
    /// Deletes an appointment by ID.
    /// </summary>
    public void Delete(int id)
    {
        var appointment = Read(id);
        if (appointment != null)
            _appointments.Remove(appointment);
    }
}
