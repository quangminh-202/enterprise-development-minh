using Polyclinic.Domain.Models;
using Polyclinic.Infrastructure.InMemory;

namespace Polyclinic.Application;

public class PatientService(IRepository<Patient, int> repo)
{
    public List<Patient> GetAll() => repo.ReadAll();
    public Patient? Get(int id) => repo.Read(id);
    public void Create(Patient p) => repo.Create(p);
    public void Update(Patient p) => repo.Update(p);
    public void Delete(int id) => repo.Delete(id);
}
