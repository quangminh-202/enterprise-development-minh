using Polyclinic.Domain.Models;
using Polyclinic.Infrastructure.InMemory;

namespace Polyclinic.Application;

public class AppointmentService(IRepository<Appointment, int> repo)
{
    public List<Appointment> GetAll() => repo.ReadAll();
    public Appointment? Get(int id) => repo.Read(id);
    public void Create(Appointment a) => repo.Create(a);
    public void Update(Appointment a) => repo.Update(a);
    public void Delete(int id) => repo.Delete(id);
}
