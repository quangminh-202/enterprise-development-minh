using Polyclinic.Domain.Models;

namespace Polyclinic.Infrastructure.InMemory;

public class PatientInMemoryRepository : IRepository<Patient, int>
{
    private readonly List<Patient> _patients = [];

    public void Create(Patient entity)
    {
        entity.Id = _patients.Count == 0 ? 1 : _patients.Max(p => p.Id) + 1;
        _patients.Add(entity);
    }

    public Patient? Read(int id) => _patients.FirstOrDefault(p => p.Id == id);
    public List<Patient> ReadAll() => [.. _patients];

    public void Update(Patient entity)
    {
        var existing = Read(entity.Id);
        if (existing != null)
        {
            _patients.Remove(existing);
            _patients.Add(entity);
        }
    }

    public void Delete(int id)
    {
        var patient = Read(id);
        if (patient != null) _patients.Remove(patient);
    }
}
