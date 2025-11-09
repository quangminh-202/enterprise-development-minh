using AutoMapper;
using Polyclinic.Application.Contracts;
using Polyclinic.Application.Interfaces;
using Polyclinic.Domain.Models;
using Polyclinic.Domain.Interfaces;

namespace Polyclinic.Application.Services;

public class DoctorService(IRepository<Doctor, int> repo, IMapper mapper) : IDoctorService
{
    public List<DoctorDto> GetAll() => mapper.Map<List<DoctorDto>>(repo.ReadAll());
    
    public DoctorDto? Get(int id) => mapper.Map<DoctorDto?>(repo.Read(id));
    
    public DoctorDto Create(CreateUpdateDoctorDto dto)
    {
        var doctor = mapper.Map<Doctor>(dto);
        var createdDoctor = repo.Create(doctor);
        return mapper.Map<DoctorDto>(createdDoctor);
    }
   
    public DoctorDto Update(int id, CreateUpdateDoctorDto dto)
    {
        var existingDoctor = repo.Read(id)
            ?? throw new ArgumentException($"Doctor with ID {id} not found.");
            
        mapper.Map(dto, existingDoctor);
        var updatedDoctor = repo.Update(existingDoctor);
        return mapper.Map<DoctorDto>(updatedDoctor);
    }
    
    public bool Delete(int id) => repo.Delete(id);
}
