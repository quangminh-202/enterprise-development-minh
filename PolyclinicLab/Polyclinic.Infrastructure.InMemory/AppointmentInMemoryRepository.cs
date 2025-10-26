using Polyclinic.Domain.Models;
using Polyclinic.Domain.Interfaces;

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
    /// Adds a new appointment to memory and returns the created appointment with its assigned ID.
    /// </summary>
    public Appointment Create(Appointment entity)
    {
        entity.Id = _appointments.Count == 0 ? 1 : _appointments.Max(a => a.Id) + 1;
        _appointments.Add(entity);
        return entity;
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
    /// Updates an existing appointment and returns the updated appointment.
    /// </summary>
    public Appointment Update(Appointment entity)
    {
        var existing = Read(entity.Id);
        if (existing != null)
        {
            _appointments.Remove(existing);
            _appointments.Add(entity);
        }
        return entity;
    }

    /// <summary>
    /// Deletes an appointment by ID. Returns true if deletion was successful, false if appointment was not found.
    /// </summary>
    public bool Delete(int id)
    {
        var appointment = Read(id);
        if (appointment != null)
        {
            _appointments.Remove(appointment);
            return true;
        }
        return false;
    }
}
