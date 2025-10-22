using Polyclinic.Domain.Models;
using Polyclinic.Infrastructure.InMemory;

namespace Polyclinic.Application;

public class DoctorService(IRepository<Doctor, int> repo)
{
    public List<Doctor> GetAll() => repo.ReadAll();
    public Doctor? Get(int id) => repo.Read(id);
    public void Create(Doctor d) => repo.Create(d);
    public void Update(Doctor d) => repo.Update(d);
    public void Delete(int id) => repo.Delete(id);
}
