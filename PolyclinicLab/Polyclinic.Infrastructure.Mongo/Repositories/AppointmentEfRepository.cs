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
        var appointment = ctx.Appointments.FirstOrDefault(a => a.Id == id);
        if (appointment != null)
        {
            appointment.Patient = ctx.Patients.FirstOrDefault(p => p.Id == appointment.PatientId)!;
            appointment.Doctor = ctx.Doctors.FirstOrDefault(d => d.Id == appointment.DoctorId)!;
        }
        return appointment;
    }

    public List<Appointment> ReadAll()
    {
        var appointments = ctx.Appointments.ToList();
        var patientIds = appointments.Select(a => a.PatientId).Distinct().ToList();
        var doctorIds = appointments.Select(a => a.DoctorId).Distinct().ToList();
        
        var patients = ctx.Patients.Where(p => patientIds.Contains(p.Id)).ToDictionary(p => p.Id);
        var doctors = ctx.Doctors.Where(d => doctorIds.Contains(d.Id)).ToDictionary(d => d.Id);
        
        foreach (var appointment in appointments)
        {
            appointment.Patient = patients.GetValueOrDefault(appointment.PatientId);
            appointment.Doctor = doctors.GetValueOrDefault(appointment.DoctorId);
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
