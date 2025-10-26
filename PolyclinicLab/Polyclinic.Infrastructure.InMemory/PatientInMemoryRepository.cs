using Polyclinic.Domain.Models;
using Polyclinic.Domain.Interfaces;

namespace Polyclinic.Infrastructure.InMemory;

public class PatientInMemoryRepository : IRepository<Patient, int>
{
    /// <summary>
    /// In-memory repository for managing patients. Supports basic CRUD operations.
    /// </summary>
    private readonly List<Patient> _patients = [];

    /// <summary>
    /// Adds a new patient to memory and returns the created patient with its assigned ID.
    /// </summary>
    public Patient Create(Patient entity)
    {
        entity.Id = _patients.Count == 0 ? 1 : _patients.Max(p => p.Id) + 1;
        _patients.Add(entity);
        return entity;
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
    /// Updates an existing patient and returns the updated patient.
    /// </summary>
    public Patient Update(Patient entity)
    {
        var existing = Read(entity.Id);
        if (existing != null)
        {
            _patients.Remove(existing);
            _patients.Add(entity);
        }
        return entity;
    }

    /// <summary>
    /// Deletes a patient by ID. Returns true if deletion was successful, false if patient was not found.
    /// </summary>
    public bool Delete(int id)
    {
        var patient = Read(id);
        if (patient != null)
        {
            _patients.Remove(patient);
            return true;
        }
        return false;
    }
}
