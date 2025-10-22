using Polyclinic.Domain.Models;

namespace Polyclinic.Infrastructure.InMemory;

/// <summary>
/// In-memory repository implementation for managing doctors.
/// Provides basic CRUD operations.
/// </summary>
public class DoctorInMemoryRepository : IRepository<Doctor, int>
{
    private readonly List<Doctor> _doctors = [];

    /// <summary>
    /// Creates a new doctor record in memory.
    /// </summary>
    public void Create(Doctor entity)
    {
        entity.Id = _doctors.Count == 0 ? 1 : _doctors.Max(d => d.Id) + 1;
        _doctors.Add(entity);
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
    /// Updates an existing doctor record.
    /// </summary>
    public void Update(Doctor entity)
    {
        var existing = Read(entity.Id);
        if (existing != null)
        {
            _doctors.Remove(existing);
            _doctors.Add(entity);
        }
    }

    /// <summary>
    /// Deletes a doctor record by ID.
    /// </summary>
    public void Delete(int id)
    {
        var doctor = Read(id);
        if (doctor != null)
            _doctors.Remove(doctor);
    }
}
