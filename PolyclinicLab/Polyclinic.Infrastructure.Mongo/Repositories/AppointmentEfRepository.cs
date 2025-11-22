using Microsoft.EntityFrameworkCore;
using Polyclinic.Domain.Interfaces;
using Polyclinic.Domain.Models;

namespace Polyclinic.Infrastructure.Mongo.Repositories;

/// <summary>
/// EF Core repository for managing appointments in MongoDB.
/// </summary>
public class AppointmentEfRepository(PolyclinicDbContext ctx) : IRepository<Appointment, int>
{
    public Appointment Create(Appointment entity)
    {
        if (entity.Id == 0)
        {
            var lastId = ctx.Appointments
                .OrderByDescending(a => a.Id)
                .Select(a => a.Id)
                .FirstOrDefault();

            entity.Id = lastId + 1;
        }

        ctx.Add(entity);
        ctx.SaveChanges();
        return entity;
    }

    public Appointment? Read(int id)
    {
        // Try using Include first (may not work with MongoDB provider)
        var appointment = ctx.Appointments
            .Where(a => a.Id == id)
            .FirstOrDefault();
            
        if (appointment != null)
        {
            // Manually load navigation properties
            if (appointment.PatientId > 0)
            {
                appointment.Patient = ctx.Patients.FirstOrDefault(p => p.Id == appointment.PatientId);
            }
            if (appointment.DoctorId > 0)
            {
                appointment.Doctor = ctx.Doctors.FirstOrDefault(d => d.Id == appointment.DoctorId);
            }
        }
        return appointment;
    }

    public List<Appointment> ReadAll()
    {
        var appointments = ctx.Appointments.ToList();
        
        if (appointments.Count > 0)
        {
            // Get all patients and doctors from DB
            var allPatients = ctx.Patients.ToList();
            var allDoctors = ctx.Doctors.ToList();
            
            // Create dictionaries for fast lookup
            var patientDict = allPatients.ToDictionary(p => p.Id);
            var doctorDict = allDoctors.ToDictionary(d => d.Id);
            
            // Manually assign navigation properties (EF Core MongoDB provider doesn't auto-load them)
            foreach (var appointment in appointments)
            {
                if (appointment.PatientId > 0 && patientDict.ContainsKey(appointment.PatientId))
                {
                    appointment.Patient = patientDict[appointment.PatientId];
                }
                
                if (appointment.DoctorId > 0 && doctorDict.ContainsKey(appointment.DoctorId))
                {
                    appointment.Doctor = doctorDict[appointment.DoctorId];
                }
            }
        }
        
        return appointments;
    }

    public Appointment Update(Appointment entity)
    {
        ctx.Update(entity);
        ctx.SaveChanges();
        return entity;
    }

    public bool Delete(int id)
    {
        var existing = ctx.Appointments.FirstOrDefault(a => a.Id == id);
        if (existing is null)
            return false;

        ctx.Remove(existing);
        ctx.SaveChanges();
        return true;
    }
}