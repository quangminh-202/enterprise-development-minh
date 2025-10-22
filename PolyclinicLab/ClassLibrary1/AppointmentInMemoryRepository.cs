using Polyclinic.Domain.Models;

namespace Polyclinic.Infrastructure.InMemory;

public class AppointmentInMemoryRepository : IRepository<Appointment, int>
{
    private readonly List<Appointment> _appointments = [];

    public void Create(Appointment entity)
    {
        entity.Id = _appointments.Count == 0 ? 1 : _appointments.Max(a => a.Id) + 1;
        _appointments.Add(entity);
    }

    public Appointment? Read(int id) => _appointments.FirstOrDefault(a => a.Id == id);
    public List<Appointment> ReadAll() => [.. _appointments];

    public void Update(Appointment entity)
    {
        var existing = Read(entity.Id);
        if (existing != null)
        {
            _appointments.Remove(existing);
            _appointments.Add(entity);
        }
    }

    public void Delete(int id)
    {
        var appt = Read(id);
        if (appt != null) _appointments.Remove(appt);
    }
}
