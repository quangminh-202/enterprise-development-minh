using AutoMapper;
using Polyclinic.Application.Contracts;
using Polyclinic.Domain.Models;
using Polyclinic.Infrastructure.InMemory;

namespace Polyclinic.Application.Services;

public class PatientService(IRepository<Patient, int> repo, IMapper mapper)
{
    public List<PatientDto> GetAll() => mapper.Map<List<PatientDto>>(repo.ReadAll());
    public PatientDto? Get(int id) => mapper.Map<PatientDto?>(repo.Read(id));
    public void Create(PatientDto dto) => repo.Create(mapper.Map<Patient>(dto));
    public void Update(PatientDto dto) => repo.Update(mapper.Map<Patient>(dto));
    public void Delete(int id) => repo.Delete(id);
}
