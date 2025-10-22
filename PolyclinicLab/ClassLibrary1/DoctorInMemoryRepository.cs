using Polyclinic.Domain.Models;

namespace Polyclinic.Infrastructure.InMemory;

public class DoctorInMemoryRepository : IRepository<Doctor, int>
{
    private readonly List<Doctor> _doctors = [];

    public void Create(Doctor entity)
    {
        entity.Id = _doctors.Count == 0 ? 1 : _doctors.Max(d => d.Id) + 1;
        _doctors.Add(entity);
    }

    public Doctor? Read(int id) => _doctors.FirstOrDefault(d => d.Id == id);

    public List<Doctor> ReadAll() => [.. _doctors];

    public void Update(Doctor entity)
    {
        var existing = Read(entity.Id);
        if (existing != null)
        {
            _doctors.Remove(existing);
            _doctors.Add(entity);
        }
    }

    public void Delete(int id)
    {
        var doctor = Read(id);
        if (doctor != null) _doctors.Remove(doctor);
    }
}
