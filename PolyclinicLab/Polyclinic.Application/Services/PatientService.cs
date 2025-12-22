using AutoMapper;
using Polyclinic.Application.Contracts;
using Polyclinic.Application.Interfaces;
using Polyclinic.Domain.Models;
using Polyclinic.Domain.Interfaces;

namespace Polyclinic.Application.Services;

public class PatientService(
    IRepository<Patient, int> repo, 
    IRepository<Appointment, int> appointmentRepo,
    IMapper mapper) : IPatientService
{
    public List<PatientDto> GetAll() => mapper.Map<List<PatientDto>>(repo.ReadAll());
    
    public PatientDto? Get(int id) => mapper.Map<PatientDto?>(repo.Read(id));
    
    public PatientDto Create(CreateUpdatePatientDto dto)
    {
        var patient = mapper.Map<Patient>(dto);
        var createdPatient = repo.Create(patient);
        return mapper.Map<PatientDto>(createdPatient);
    }
    
    public PatientDto Update(int id, CreateUpdatePatientDto dto)
    {
        var existingPatient = repo.Read(id)
           ?? throw new ArgumentException($"Patient with ID {id} not found.");
            
        mapper.Map(dto, existingPatient);
        var updatedPatient = repo.Update(existingPatient);
        return mapper.Map<PatientDto>(updatedPatient);
    }
    
    public bool Delete(int id)
    {
        // First, delete all appointments for this patient
        var appointments = appointmentRepo.ReadAll().Where(a => a.PatientId == id).ToList();
        foreach (var appointment in appointments)
        {
            appointmentRepo.Delete(appointment.Id);
        }
        
        // Then delete the patient
        return repo.Delete(id);
    }
}
