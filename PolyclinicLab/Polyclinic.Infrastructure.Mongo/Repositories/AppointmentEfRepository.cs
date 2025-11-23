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
            LoadNavigationProperties(appointment);
        return appointment;
    }

    public List<Appointment> ReadAll()
    {
        var appointments = ctx.Appointments.ToList();
        if (appointments.Count == 0) return appointments;

        var patientDict = ctx.Patients.ToList().ToDictionary(p => p.Id);
        var doctorDict = ctx.Doctors.ToList().ToDictionary(d => d.Id);

        foreach (var appointment in appointments)
        {
            if (patientDict.TryGetValue(appointment.PatientId, out var patient))
                appointment.Patient = patient;
            if (doctorDict.TryGetValue(appointment.DoctorId, out var doctor))
                appointment.Doctor = doctor;
        }

        return appointments;
    }

    private void LoadNavigationProperties(Appointment appointment)
    {
        if (appointment.PatientId > 0)
            appointment.Patient = ctx.Patients.FirstOrDefault(p => p.Id == appointment.PatientId);
        if (appointment.DoctorId > 0)
            appointment.Doctor = ctx.Doctors.FirstOrDefault(d => d.Id == appointment.DoctorId);
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