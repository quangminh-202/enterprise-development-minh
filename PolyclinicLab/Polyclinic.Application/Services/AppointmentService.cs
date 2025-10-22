using AutoMapper;
using Polyclinic.Application.Contracts;
using Polyclinic.Domain.Models;
using Polyclinic.Infrastructure.InMemory;

namespace Polyclinic.Application.Services;

public class AppointmentService(IRepository<Appointment, int> repo, IMapper mapper)
{
    public List<AppointmentDto> GetAll() => mapper.Map<List<AppointmentDto>>(repo.ReadAll());
    public AppointmentDto? Get(int id) => mapper.Map<AppointmentDto?>(repo.Read(id));
    public void Create(AppointmentDto dto) => repo.Create(mapper.Map<Appointment>(dto));
    public void Update(AppointmentDto dto) => repo.Update(mapper.Map<Appointment>(dto));
    public void Delete(int id) => repo.Delete(id);
}
