using Polyclinic.Domain.Models;
using Polyclinic.Domain.Interfaces;

namespace Polyclinic.Infrastructure.InMemory;

/// <summary>
/// In-memory repository implementation for managing doctors.
/// Provides basic CRUD operations.
/// </summary>
public class DoctorInMemoryRepository : IRepository<Doctor, int>
{
    /// <summary>
    /// In-memory collection storing doctor data.
    /// </summary>
    private readonly List<Doctor> _doctors = [];

    /// <summary>
    /// Creates a new doctor record in memory and returns the created doctor with its assigned ID.
    /// </summary>
    public Doctor Create(Doctor entity)
    {
        entity.Id = _doctors.Count == 0 ? 1 : _doctors.Max(d => d.Id) + 1;
        _doctors.Add(entity);
        return entity;
    }

    /// <summary>
    /// Reads a doctor by their unique ID.
    /// </summary>
    public Doctor? Read(int id) => _doctors.FirstOrDefault(d => d.Id == id);

    /// <summary>
    /// Returns all doctors currently stored in memory.
    /// </summary>
    public List<Doctor> ReadAll() => [.. _doctors];

    /// <summary>
    /// Updates an existing doctor record and returns the updated doctor.
    /// </summary>
    public Doctor Update(Doctor entity)
    {
        var existing = Read(entity.Id);
        if (existing != null)
        {
            _doctors.Remove(existing);
            _doctors.Add(entity);
        }
        return entity;
    }

    /// <summary>
    /// Deletes a doctor record by ID. Returns true if deletion was successful, false if doctor was not found.
    /// </summary>
    public bool Delete(int id)
    {
        var doctor = Read(id);
        if (doctor != null)
        {
            _doctors.Remove(doctor);
            return true;
        }
        return false;
    }
}
