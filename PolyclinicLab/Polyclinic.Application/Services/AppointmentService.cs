using AutoMapper;
using Polyclinic.Application.Contracts;
using Polyclinic.Application.Interfaces;
using Polyclinic.Domain.Models;
using Polyclinic.Domain.Interfaces;
using Polyclinic.Infrastructure.InMemory;

namespace Polyclinic.Application.Services;

public class AppointmentService(
    IRepository<Appointment, int> appointmentRepo, 
    IRepository<Doctor, int> doctorRepo,
    IRepository<Patient, int> patientRepo,
    IMapper mapper) : IAppointmentService
{
    public List<AppointmentDto> GetAll() => mapper.Map<List<AppointmentDto>>(appointmentRepo.ReadAll());
    
    public AppointmentDto? Get(int id) => mapper.Map<AppointmentDto?>(appointmentRepo.Read(id));
    
    public AppointmentDto Create(CreateUpdateAppointmentDto dto)
    {
        var appointment = mapper.Map<Appointment>(dto);
        
        // Set Doctor and Patient references
        var doctor = doctorRepo.Read(dto.DoctorId);
        var patient = patientRepo.Read(dto.PatientId);
        
        if (doctor == null)
            throw new ArgumentException($"Doctor with ID {dto.DoctorId} not found.");
        if (patient == null)
            throw new ArgumentException($"Patient with ID {dto.PatientId} not found.");
            
        appointment.Doctor = doctor;
        appointment.Patient = patient;
        
        var createdAppointment = appointmentRepo.Create(appointment);
        return mapper.Map<AppointmentDto>(createdAppointment);
    }
    
    public AppointmentDto Update(int id, CreateUpdateAppointmentDto dto)
    {
        var existingAppointment = appointmentRepo.Read(id);
        if (existingAppointment == null)
            throw new ArgumentException($"Appointment with ID {id} not found.");
            
        // Set Doctor and Patient references
        var doctor = doctorRepo.Read(dto.DoctorId);
        var patient = patientRepo.Read(dto.PatientId);
        
        if (doctor == null)
            throw new ArgumentException($"Doctor with ID {dto.DoctorId} not found.");
        if (patient == null)
            throw new ArgumentException($"Patient with ID {dto.PatientId} not found.");
            
        mapper.Map(dto, existingAppointment);
        existingAppointment.Doctor = doctor;
        existingAppointment.Patient = patient;
        
        var updatedAppointment = appointmentRepo.Update(existingAppointment);
        return mapper.Map<AppointmentDto>(updatedAppointment);
    }
    
    public bool Delete(int id) => appointmentRepo.Delete(id);
}
