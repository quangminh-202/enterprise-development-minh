using Polyclinic.Domain.Models;
using Polyclinic.Domain.Interfaces;
using Polyclinic.Domain.Data;

namespace Polyclinic.Infrastructure.InMemory;

/// <summary>
/// Provides a way to seed in-memory repositories with initial data
/// from the PolyclinicFixture.
/// </summary>
/// <remarks>
/// Register this class as Singleton in the DI container.
/// </remarks>
public class DataSeeder(
    IRepository<Doctor, int> doctorRepo,
    IRepository<Patient, int> patientRepo,
    IRepository<Appointment, int> appointmentRepo)
{
    /// <summary>
    /// Seeds the database with initial data.
    /// </summary>
    public void Seed()
    {
        var fixture = new PolyclinicFixture();

        // Create all doctors in the repository
        foreach (var doctor in fixture.Doctors)
            doctorRepo.Create(doctor);

        // Create all patients in the repository
        foreach (var patient in fixture.Patients)
            patientRepo.Create(patient);

        // Create appointments and retrieve related entities from the repositories
        foreach (var a in fixture.Appointments)
        {
            // Retrieve doctor and patient by their IDs from the repository
            var doctor = doctorRepo.Read(a.Doctor.Id);
            var patient = patientRepo.Read(a.Patient.Id);
            if (doctor is null || patient is null)
                continue;

            // Create a new appointment with proper references
            appointmentRepo.Create(new Appointment
            {
                Date = a.Date,
                Room = a.Room,
                IsRepeated = a.IsRepeated,
                DoctorId = doctor.Id,
                PatientId = patient.Id,
                Doctor = doctor,
                Patient = patient
            });
        }
    }
}