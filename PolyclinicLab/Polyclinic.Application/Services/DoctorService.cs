using AutoMapper;
using Polyclinic.Application.Contracts;
using Polyclinic.Domain.Models;
using Polyclinic.Infrastructure.InMemory;

namespace Polyclinic.Application.Services;

public class DoctorService(IRepository<Doctor, int> repo, IMapper mapper)
{
    public List<DoctorDto> GetAll() => mapper.Map<List<DoctorDto>>(repo.ReadAll());
    public DoctorDto? Get(int id) => mapper.Map<DoctorDto?>(repo.Read(id));
    public void Create(DoctorDto dto) => repo.Create(mapper.Map<Doctor>(dto));
    public void Update(DoctorDto dto) => repo.Update(mapper.Map<Doctor>(dto));
    public void Delete(int id) => repo.Delete(id);
}
