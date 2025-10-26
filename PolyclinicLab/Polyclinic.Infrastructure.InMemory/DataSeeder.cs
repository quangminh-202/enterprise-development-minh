using Polyclinic.Domain.Models;
using Polyclinic.Domain.Interfaces;
using Polyclinic.Domain.Data;

namespace Polyclinic.Infrastructure.InMemory;

/// <summary>
/// Provides a way to seed in-memory repositories with initial data
/// from the PolyclinicFixture.
/// </summary>
public static class DataSeeder
{
    public static void Seed(
        IRepository<Doctor, int> doctorRepo,
        IRepository<Patient, int> patientRepo,
        IRepository<Appointment, int> appointmentRepo)
    {
        var fixture = new PolyclinicFixture();

        // Seed Doctors and get them back with IDs
        var doctors = new List<Doctor>();
        foreach (var d in fixture.Doctors)
            doctors.Add(doctorRepo.Create(d));

        // Seed Patients and get them back with IDs
        var patients = new List<Patient>();
        foreach (var p in fixture.Patients)
            patients.Add(patientRepo.Create(p));

        // Seed Appointments
        foreach (var a in fixture.Appointments)
        {
            // Find the actual doctor and patient from the seeded lists
            var doctor = doctors.First(d => d.Passport == a.Doctor.Passport);
            var patient = patients.First(p => p.Passport == a.Patient.Passport);
            
            // Create a new appointment with proper foreign key references
            var appointment = new Appointment
            {
                Date = a.Date,
                Room = a.Room,
                IsRepeated = a.IsRepeated,
                DoctorId = doctor.Id,
                PatientId = patient.Id,
                Doctor = doctor,
                Patient = patient
            };
            appointmentRepo.Create(appointment);
        }
    }
}
