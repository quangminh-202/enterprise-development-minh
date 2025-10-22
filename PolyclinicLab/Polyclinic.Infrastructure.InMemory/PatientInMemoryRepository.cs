using Polyclinic.Domain.Models;

namespace Polyclinic.Infrastructure.InMemory;

public class PatientInMemoryRepository : IRepository<Patient, int>
{
    /// <summary>
    /// In-memory repository for managing patients. Supports basic CRUD operations.
    /// </summary>
    private readonly List<Patient> _patients = [];

    /// <summary>
    /// Adds a new patient to memory.
    /// </summary>
    public void Create(Patient entity)
    {
        entity.Id = _patients.Count == 0 ? 1 : _patients.Max(p => p.Id) + 1;
        _patients.Add(entity);
    }

    /// <summary>
    /// Finds a patient by ID.
    /// </summary>
    public Patient? Read(int id) => _patients.FirstOrDefault(p => p.Id == id);

    /// <summary>
    /// Returns all patients.
    /// </summary>
    public List<Patient> ReadAll() => [.. _patients];


    /// <summary>
    /// Updates an existing patient.
    /// </summary>
    public void Update(Patient entity)
    {
        var existing = Read(entity.Id);
        if (existing != null)
        {
            _patients.Remove(existing);
            _patients.Add(entity);
        }
    }

    /// <summary>
    /// Deletes a patient by ID.
    /// </summary>
    public void Delete(int id)
    {
        var patient = Read(id);
        if (patient != null) _patients.Remove(patient);
    }
}
